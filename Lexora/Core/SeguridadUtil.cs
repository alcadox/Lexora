using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lexora.Core
{
    public static class SeguridadUtil
    {
        public static string CalcularHashSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(texto);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        // ---HASH DE ARCHIVOS REALES (NO TEXTO) ---
        public static string CalcularHashArchivoSHA256(string ruta)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Abrimos el archivo en modo solo lectura para no bloquearlo
                using (FileStream fs = File.OpenRead(ruta))
                {
                    byte[] hash = sha256.ComputeHash(fs);
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hash) sb.Append(b.ToString("x2"));
                    return sb.ToString();
                }
            }
        }

    }
}