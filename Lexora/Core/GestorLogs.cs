using Npgsql;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Data;

namespace Lexora.Core
{
    public static class GestorLogs
    {
        // Esta variable se debe asignar cuando el usuario hace login (ej: GestorLogs.IdUsuarioActual = id;)
        // Si es nulo o 0, significa que NO hay sesión iniciada y abortará el log.
        public static int? IdUsuarioActual { get; set; }

        private static string ObtenerConexion() => ConfigurationManager.ConnectionStrings["conexionDBLexora"].ConnectionString;

        public static void Registrar(string tipoAccion, string descripcionResumida)
        {
            try
            {
                if (!IdUsuarioActual.HasValue || IdUsuarioActual.Value <= 0) return;

                int idUsuario = IdUsuarioActual.Value;
                string nombreEquipo = Environment.MachineName;
                string descripcionCifrada = SeguridadAvanzadaUtil.CifrarTextoLog(descripcionResumida);

                // Disparamos la inserción en el hilo secundario
                Task.Run(() =>
                {
                    // Obtenemos la IP dentro del hilo secundario para que el retraso de red (ping) 
                    // no congele ni un milisegundo la interfaz del usuario.
                    string ipAddress = ObtenerIPPuntoAcceso();
                    InsertarLogEnBD(idUsuario, tipoAccion, descripcionCifrada, ipAddress, nombreEquipo);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOG DESCARTADO] Error interno: {ex.Message}");
            }
        }

        // --- RASTREO DE IP ---
        private static string ObtenerIPPuntoAcceso()
        {
            try
            {
                // 1. INTENTO PRIMARIO: Obtener la IP Pública real
                using (var client = new HttpClient())
                {
                    // Le damos solo 3 segundos. Si la red va lenta, abortamos para no quedar colgados.
                    client.Timeout = TimeSpan.FromSeconds(3);

                    // ipify es un servicio gratuito, ultrarrápido y estándar en ciberseguridad para esto
                    string ipPublica = client.GetStringAsync("https://api.ipify.org").GetAwaiter().GetResult();
                    return ipPublica.Trim();
                }
            }
            catch
            {
                // 2. FALLBACK MILITAR (PLAN B): Si no hay internet o falla el servidor, 
                // capturamos la IP Privada local en lugar de dejar el campo vacío.
                return ObtenerIPLocalFalsa();
            }
        }

        private static string ObtenerIPLocalFalsa()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return "LOCAL:" + ip.ToString(); // Le ponemos "LOCAL:" para saber en DB que falló la red
                }
            }
            catch { }
            return "OFFLINE";
        }

        private static void InsertarLogEnBD(int idUsuario, string tipoAccion, string descripcionCifrada, string ip, string equipo)
        {
            try
            {
                using (var conn = new NpgsqlConnection(ObtenerConexion()))
                {
                    conn.Open();
                    //Añadimos la columna 'fecha' a la consulta para sobrescribir el DEFAULT NOW()
                    string sql = @"INSERT INTO log (id_usuario, tipo_accion, descripcion, ip_address, nombre_equipo, fecha) 
                           VALUES (@id_usuario, @accion, @descripcion, @ip, @equipo, @fecha);";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                        cmd.Parameters.AddWithValue("@accion", tipoAccion);
                        cmd.Parameters.AddWithValue("@descripcion", descripcionCifrada);
                        cmd.Parameters.AddWithValue("@ip", ip);
                        cmd.Parameters.AddWithValue("@equipo", equipo);

                        // Obligamos a la BD a guardar la hora exacta (del PC del usuario)
                        cmd.Parameters.AddWithValue("@fecha", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR CRÍTICO LOG] {ex.Message}");
            }
        }

        public static DataTable ObtenerHistorialLogs(int idUsuario)
        {
            GestorLogs.Registrar("CONSULTA_LOGS", $"El usuario consultó su historial de logs.");
            DataTable dt = new DataTable();
            // Definimos las columnas que verá el usuario en la tabla
            dt.Columns.Add("Fecha", typeof(string));
            dt.Columns.Add("Acción", typeof(string));
            dt.Columns.Add("Descripción Detallada", typeof(string));
            dt.Columns.Add("Dirección IP", typeof(string));

            try
            {
                using (var conn = new NpgsqlConnection(ObtenerConexion()))
                {
                    conn.Open();
                    // Traemos los últimos 100 registros ordenados del más nuevo al más viejo
                    string sql = @"SELECT fecha, tipo_accion, descripcion, ip_address 
                           FROM log 
                           WHERE id_usuario = @id_usuario 
                           ORDER BY fecha DESC LIMIT 100;";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // 1. Extraemos los datos crudos
                                string fecha = reader.GetDateTime(0).ToString("dd/MM/yyyy HH:mm");
                                string accion = reader.GetString(1);
                                string descripcionCifrada = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                string ip = reader.GetString(3);

                                // 2. DESCIFRADO MILITAR AL VUELO
                                string descripcionDescifrada = "";
                                try
                                {
                                    descripcionDescifrada = SeguridadAvanzadaUtil.DescifrarTextoLog(descripcionCifrada);
                                }
                                catch
                                {
                                    // Blindaje: Si un atacante modificó un log a mano en la BD y rompió el Base64/IV,
                                    // capturamos el error para que la tabla no crashee entera.
                                    descripcionDescifrada = "[REGISTRO CORRUPTO O ALTERADO]";
                                }

                                // 3. Añadimos la fila ya limpia
                                dt.Rows.Add(fecha, accion, descripcionDescifrada, ip);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al extraer logs: {ex.Message}");
                throw new Exception("No se pudo conectar de forma segura con el registro de auditoría.");
            }

            return dt;
        }
    }
}