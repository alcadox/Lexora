using Npgsql;
using System;
using System.Configuration;

namespace Lexora.Core
{
    public class DatosUsuarioAuth
    {
        public bool Existe { get; set; }
        public bool Activo { get; set; }
        public string Nombre { get; set; }
        public string HashBD { get; set; }
    }

    public static class GestorDBAuth
    {
        private static string ObtenerConexion() => ConfigurationManager.ConnectionStrings["conexionDBLexora"].ConnectionString;

        public static DatosUsuarioAuth ObtenerDatosUsuario(string email)
        {
            var resultado = new DatosUsuarioAuth { Existe = false };

            using (var conn = new NpgsqlConnection(ObtenerConexion()))
            {
                conn.Open();
                string sql = "SELECT nombre, contrasena_hash, activo FROM usuario WHERE email = @email;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resultado.Existe = true;
                            resultado.Activo = reader.GetBoolean(reader.GetOrdinal("activo"));
                            resultado.HashBD = reader.GetString(reader.GetOrdinal("contrasena_hash"));
                            resultado.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                        }
                    }
                }
            }
            return resultado;
        }

        public static bool ActualizarContrasena(string email, string nuevaContrasenaHash)
        {
            using (var conn = new NpgsqlConnection(ObtenerConexion()))
            {
                conn.Open();
                string sql = "UPDATE usuario SET contrasena_hash = @contrasena_hash WHERE email = @email;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@contrasena_hash", nuevaContrasenaHash);
                    cmd.Parameters.AddWithValue("@email", email);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}