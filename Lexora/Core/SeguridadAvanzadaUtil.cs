using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lexora.Core
{
    public static class SeguridadAvanzadaUtil
    {
        // --- 1. CIFRADO AES-256 ---
        public static void CifrarArchivo(string ruta, string password)
        {
            string rutaSalida = ruta + ".lxr";
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 50000))
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Key = pbkdf2.GetBytes(32);
                aes.IV = pbkdf2.GetBytes(16);

                using (var fsOut = new FileStream(rutaSalida, FileMode.Create))
                {
                    fsOut.Write(salt, 0, salt.Length); // Guardamos la sal al principio
                    using (var cryptoStream = new CryptoStream(fsOut, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var fsIn = new FileStream(ruta, FileMode.Open))
                    {
                        fsIn.CopyTo(cryptoStream);
                    }
                }
            }
            File.Delete(ruta); // Borrar original (Opcional: usar Wipe aquí)
        }

        public static void DescifrarArchivo(string ruta, string password)
        {
            if (!ruta.EndsWith(".lxr")) throw new Exception("El archivo no es un archivo cifrado de Lexora.");
            string rutaSalida = ruta.Substring(0, ruta.Length - 4);

            byte[] salt = new byte[16];
            using (var fsIn = new FileStream(ruta, FileMode.Open))
            {
                fsIn.Read(salt, 0, salt.Length);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 50000))
                using (var aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.Key = pbkdf2.GetBytes(32);
                    aes.IV = pbkdf2.GetBytes(16);

                    using (var cryptoStream = new CryptoStream(fsIn, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var fsOut = new FileStream(rutaSalida, FileMode.Create))
                    {
                        cryptoStream.CopyTo(fsOut);
                    }
                }
            }
            File.Delete(ruta);
        }

        // --- 2. BÓVEDA LEXORA (PERMISOS ACL) ---
        public static void AlternarBloqueoCarpeta(string ruta)
        {
            string usuarioActual = Environment.UserDomainName + "\\" + Environment.UserName;
            DirectoryInfo dInfo = new DirectoryInfo(ruta);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            // Buscamos si ya está bloqueada
            bool estaBloqueada = false;
            AuthorizationRuleCollection rules = dSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (FileSystemAccessRule rule in rules)
            {
                if (rule.IdentityReference.Value.Equals(usuarioActual, StringComparison.CurrentCultureIgnoreCase) &&
                    rule.AccessControlType == AccessControlType.Deny)
                {
                    estaBloqueada = true;
                    break;
                }
            }

            if (estaBloqueada)
            {
                dSecurity.RemoveAccessRule(new FileSystemAccessRule(usuarioActual, FileSystemRights.FullControl, AccessControlType.Deny));
            }
            else
            {
                dSecurity.AddAccessRule(new FileSystemAccessRule(usuarioActual, FileSystemRights.FullControl, AccessControlType.Deny));
            }
            Directory.SetAccessControl(ruta, dSecurity);
        }

        // --- 3. OCULTACIÓN FUERTE (ROOTKIT BÁSICO) ---
        public static void AlternarOcultacionFuerte(string ruta)
        {
            FileAttributes atributos = File.GetAttributes(ruta);
            if ((atributos & FileAttributes.System) == FileAttributes.System)
            {
                // Quitar System y Hidden
                File.SetAttributes(ruta, atributos & ~FileAttributes.System & ~FileAttributes.Hidden);
            }
            else
            {
                // Poner System y Hidden
                File.SetAttributes(ruta, atributos | FileAttributes.System | FileAttributes.Hidden);
            }
        }

        // --- 4. DESTRUCCIÓN DoD 5220.22-M (3 PASADAS) ---
        public static async Task DestruccionDoD(string ruta)
        {
            await Task.Run(() =>
            {
                byte[] bufferCeros = new byte[64 * 1024];
                byte[] bufferUnos = new byte[64 * 1024];
                for (int i = 0; i < bufferUnos.Length; i++) bufferUnos[i] = 0xFF;

                byte[] bufferRandom = new byte[64 * 1024];
                var rng = new Random();

                using (FileStream fs = new FileStream(ruta, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    long length = fs.Length;

                    // Pasada 1: Ceros
                    fs.Position = 0;
                    EscribirBuffer(fs, bufferCeros, length);

                    // Pasada 2: Unos
                    fs.Position = 0;
                    EscribirBuffer(fs, bufferUnos, length);

                    // Pasada 3: Random
                    fs.Position = 0;
                    rng.NextBytes(bufferRandom);
                    EscribirBuffer(fs, bufferRandom, length);
                }

                // Renombrar el archivo antes de borrarlo para destruir el rastro del nombre
                string rutaTemp = Path.Combine(Path.GetDirectoryName(ruta), Guid.NewGuid().ToString() + ".tmp");
                File.Move(ruta, rutaTemp);
                File.Delete(rutaTemp);
            });
        }

        private static void EscribirBuffer(FileStream fs, byte[] buffer, long length)
        {
            long escritos = 0;
            while (escritos < length)
            {
                int aEscribir = (int)Math.Min(buffer.Length, length - escritos);
                fs.Write(buffer, 0, aEscribir);
                escritos += aEscribir;
            }
            fs.Flush();
        }

        // --- 5. DETECTOR DE MALWARE (HASH SCANNER VIRUSTOTAL) ---
        public static void EscanearEnVirusTotal(string ruta)
        {
            // Usamos tu utilidad ya creada
            string hash = SeguridadUtil.CalcularHashSHA256(ruta);
            Process.Start(new ProcessStartInfo($"https://www.virustotal.com/gui/file/{hash}") { UseShellExecute = true });
        }
    }
}