using Npgsql;
using System;
using System.Configuration;
using System.Security.Cryptography;

namespace Lexora.Core
{
    // Objeto de transporte de datos (DTO) ampliado con seguridad
    public class DatosUsuarioAuth
    {
        public int IdUsuario { get; set; }
        public bool Existe { get; set; }
        public bool Activo { get; set; }
        public string Nombre { get; set; }
        public string HashBD { get; set; }
        public int IntentosFallidos { get; set; }
        public DateTime? BloqueadoHasta { get; set; }
        public int PuntosIA { get; set; }
    }

    public static class GestorDBAuth
    {
        // 1. CONEXIÓN SEGURA CON SSL REQUERIDO
        // Nota: Asegúrate App.config tenga "SSL Mode=Require;Trust Server Certificate=true;"
        private static string ObtenerConexion() => ConfigurationManager.ConnectionStrings["conexionDBLexora"].ConnectionString;

        // 2. OBTENER DATOS (Ahora trae el ID y el estado de bloqueo)
        public static DatosUsuarioAuth ObtenerDatosUsuario(string email)
        {
            var resultado = new DatosUsuarioAuth { Existe = false };

            using (var conn = new NpgsqlConnection(ObtenerConexion()))
            {
                conn.Open();
                string sql = @"SELECT id_usuario, nombre, contrasena_hash, activo, 
                                      intentos_fallidos, bloqueado_hasta, ia_puntos_diarios_restantes 
                               FROM usuario WHERE email = @email;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resultado.Existe = true;
                            resultado.IdUsuario = reader.GetInt32(reader.GetOrdinal("id_usuario"));
                            resultado.Activo = reader.GetBoolean(reader.GetOrdinal("activo"));
                            resultado.HashBD = reader.GetString(reader.GetOrdinal("contrasena_hash"));
                            resultado.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                            resultado.IntentosFallidos = reader.IsDBNull(reader.GetOrdinal("intentos_fallidos")) ? 0 : reader.GetInt32(reader.GetOrdinal("intentos_fallidos"));
                            resultado.BloqueadoHasta = reader.IsDBNull(reader.GetOrdinal("bloqueado_hasta")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("bloqueado_hasta"));
                            resultado.PuntosIA = reader.IsDBNull(reader.GetOrdinal("ia_puntos_diarios_restantes")) ? 0 : reader.GetInt32(reader.GetOrdinal("ia_puntos_diarios_restantes"));
                        }
                    }
                }
            }
            return resultado;
        }

        // 3. SEGURIDAD: REGISTRAR INTENTO FALLIDO Y BLOQUEAR SI ES NECESARIO
        public static int RegistrarIntentoFallido(string email, int intentosActuales)
        {
            int nuevosIntentos = intentosActuales + 1;
            DateTime? bloqueo = null;

            // Si falla 5 veces seguidas, lo bloqueamos 15 minutos
            if (nuevosIntentos >= 5)
                bloqueo = DateTime.Now.AddMinutes(15);

            using (var conn = new NpgsqlConnection(ObtenerConexion()))
            {
                conn.Open();
                string sql = "UPDATE usuario SET intentos_fallidos = @intentos, bloqueado_hasta = @bloqueo WHERE email = @email;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@intentos", nuevosIntentos);
                    cmd.Parameters.AddWithValue("@bloqueo", (object)bloqueo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.ExecuteNonQuery();
                }
            }
            return nuevosIntentos;
        }

        // 4. SEGURIDAD: LOGIN EXITOSO (Limpia bloqueos y genera TOKEN)
        public static string RegistrarLoginExitoso(int idUsuario, string email)
        {
            // A. Limpiar el historial de fallos del usuario
            using (var conn = new NpgsqlConnection(ObtenerConexion()))
            {
                conn.Open();
                string sqlUpdate = "UPDATE usuario SET intentos_fallidos = 0, bloqueado_hasta = NULL, ultimo_login = NOW() WHERE email = @email;";
                using (var cmd = new NpgsqlCommand(sqlUpdate, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.ExecuteNonQuery();
                }

                // B. Generar un Token de Sesión Criptográficamente Seguro
                string tokenSesion = GenerarTokenSeguro();
                DateTime expiracion = DateTime.Now.AddDays(30); // El token dura 30 días

                string sqlToken = "INSERT INTO sesion_usuario (id_usuario, token_sesion, fecha_expiracion) VALUES (@id, @token, @exp);";
                using (var cmdToken = new NpgsqlCommand(sqlToken, conn))
                {
                    cmdToken.Parameters.AddWithValue("@id", idUsuario);
                    cmdToken.Parameters.AddWithValue("@token", tokenSesion);
                    cmdToken.Parameters.AddWithValue("@exp", expiracion);
                    cmdToken.ExecuteNonQuery();
                }

                return tokenSesion; // Devolvemos el token para que el frontend lo guarde
            }
        }

        // Utilidad interna para generar tokens imposibles de adivinar
        private static string GenerarTokenSeguro()
        {
            byte[] bytesAleatorios = new byte[32]; // 256 bits de entropía
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytesAleatorios);
            }
            return Convert.ToBase64String(bytesAleatorios);
        }

        // Actualizar contraseña
        public static bool ActualizarContrasena(string email, string nuevaContrasenaHash)
        {
            using (var conn = new NpgsqlConnection(ObtenerConexion()))
            {
                conn.Open();
                string sql = "UPDATE usuario SET contrasena_hash = @hash, intentos_fallidos = 0, bloqueado_hasta = NULL WHERE email = @email;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@hash", nuevaContrasenaHash);
                    cmd.Parameters.AddWithValue("@email", email);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}