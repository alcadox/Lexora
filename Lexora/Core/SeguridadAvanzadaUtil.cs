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

        // --- 4. DESTRUCCIÓN DoD 5220.22-M (3 PASADAS RECURSIVA) ---
        public static async Task DestruccionDoD(string ruta, bool esCarpeta)
        {
            await Task.Run(() =>
            {
                EjecutarWipeRecursivo(ruta, esCarpeta);
            });
        }

        private static void EjecutarWipeRecursivo(string rutaActual, bool esCarpetaActual)
        {
            if (esCarpetaActual)
            {
                // 1. RECURSIVIDAD: Trituramos el interior de la carpeta primero
                foreach (string subCarpeta in Directory.GetDirectories(rutaActual))
                {
                    EjecutarWipeRecursivo(subCarpeta, true);
                }
                foreach (string archivo in Directory.GetFiles(rutaActual))
                {
                    EjecutarWipeRecursivo(archivo, false);
                }

                // 2. Destruimos el nombre de la carpeta y la borramos
                string dirPadre = Path.GetDirectoryName(rutaActual);
                string rutaTemp = Path.Combine(dirPadre, Guid.NewGuid().ToString("N"));
                Directory.Move(rutaActual, rutaTemp);
                Directory.Delete(rutaTemp);
            }
            else
            {
                // 1. WIPE MILITAR DE ARCHIVO (3 Pasadas)
                byte[] bufferCeros = new byte[64 * 1024];
                byte[] bufferUnos = new byte[64 * 1024];
                for (int i = 0; i < bufferUnos.Length; i++) bufferUnos[i] = 0xFF;

                byte[] bufferRandom = new byte[64 * 1024];
                var rng = new Random();

                try
                {
                    using (FileStream fs = new FileStream(rutaActual, FileMode.Open, FileAccess.Write, FileShare.None))
                    {
                        long length = fs.Length;

                        // Pasada 1: Ceros
                        fs.Position = 0; EscribirBuffer(fs, bufferCeros, length);
                        // Pasada 2: Unos
                        fs.Position = 0; EscribirBuffer(fs, bufferUnos, length);
                        // Pasada 3: Random
                        fs.Position = 0; rng.NextBytes(bufferRandom); EscribirBuffer(fs, bufferRandom, length);
                    }

                    // 2. Destruimos el nombre del archivo y lo borramos
                    string dirPadre = Path.GetDirectoryName(rutaActual);
                    string rutaTemp = Path.Combine(dirPadre, Guid.NewGuid().ToString("N") + ".tmp");
                    File.Move(rutaActual, rutaTemp);
                    File.Delete(rutaTemp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al triturar {rutaActual}: {ex.Message}");
                }
            }
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
            try
            {
                // Ahora sí calculamos el hash del contenido del archivo
                string hash = SeguridadUtil.CalcularHashArchivoSHA256(ruta);
                
                // Usamos el endpoint de búsqueda (/search/) que es más robusto
                Process.Start(new ProcessStartInfo($"https://www.virustotal.com/gui/search/{hash}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error al leer el archivo para VirusTotal: {ex.Message}", "Lexora Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        // --- 6. ANIQUILACIÓN DE METADATOS (ANTI-FORENSE RECURSIVO) ---
        public static async Task AniquilarMetadatos(string ruta, bool esCarpeta)
        {
            await Task.Run(() =>
            {
                Random rng = new Random();
                EjecutarAniquilacionRecursiva(ruta, esCarpeta, rng);
            });
        }

        private static void EjecutarAniquilacionRecursiva(string rutaActual, bool esCarpetaActual, Random rng)
        {
            try
            {
                // 1. RECURSIVIDAD: Si es carpeta, nos metemos hasta el fondo primero (Bottom-Up)
                if (esCarpetaActual)
                {
                    // Aniquilamos todo el contenido interior primero
                    foreach (string subCarpeta in Directory.GetDirectories(rutaActual))
                    {
                        EjecutarAniquilacionRecursiva(subCarpeta, true, rng);
                    }
                    foreach (string archivo in Directory.GetFiles(rutaActual))
                    {
                        EjecutarAniquilacionRecursiva(archivo, false, rng);
                    }
                }

                // 2. LÓGICA DE ANIQUILACIÓN PARA EL ELEMENTO ACTUAL
                string nuevaRuta = rutaActual;

                // Renombrado Criptográfico
                string directorio = Path.GetDirectoryName(rutaActual);
                string extension = esCarpetaActual ? "" : Path.GetExtension(rutaActual).ToLowerInvariant();
                string nuevoNombre = Guid.NewGuid().ToString("N") + extension;
                nuevaRuta = Path.Combine(directorio, nuevoNombre);

                if (esCarpetaActual) Directory.Move(rutaActual, nuevaRuta);
                else File.Move(rutaActual, nuevaRuta);

                // Borrado de ADS
                if (!esCarpetaActual)
                {
                    try { File.Delete(nuevaRuta + ":Zone.Identifier"); } catch { }
                }

                // Aniquilación EXIF (Imágenes)
                if (!esCarpetaActual && (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp"))
                {
                    string rutaTemp = nuevaRuta + ".tmp";
                    using (Image imgOriginal = Image.FromFile(nuevaRuta))
                    using (Bitmap copiaVirgen = new Bitmap(imgOriginal.Width, imgOriginal.Height))
                    {
                        using (Graphics g = Graphics.FromImage(copiaVirgen))
                        {
                            g.DrawImage(imgOriginal, 0, 0, imgOriginal.Width, imgOriginal.Height);
                        }
                        copiaVirgen.Save(rutaTemp, imgOriginal.RawFormat);
                    }
                    File.Delete(nuevaRuta);
                    File.Move(rutaTemp, nuevaRuta);
                }

                // Data Padding (Ruido binario al archivo para mutar el peso y el Hash)
                if (!esCarpetaActual)
                {
                    try
                    {
                        int bytesExtra = rng.Next(1024, 51200);
                        byte[] ruido = new byte[bytesExtra];
                        rng.NextBytes(ruido);
                        using (var stream = new FileStream(nuevaRuta, FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            stream.Write(ruido, 0, ruido.Length);
                        }
                    }
                    catch { }
                }

                // Fechas Falsas (Años 90s - 2005)
                DateTime fechaFalsa = new DateTime(rng.Next(1990, 2005), rng.Next(1, 13), rng.Next(1, 28), rng.Next(0, 24), rng.Next(0, 60), rng.Next(0, 60));

                if (esCarpetaActual)
                {
                    Directory.SetCreationTime(nuevaRuta, fechaFalsa);
                    Directory.SetLastWriteTime(nuevaRuta, fechaFalsa);
                    Directory.SetLastAccessTime(nuevaRuta, fechaFalsa);
                }
                else
                {
                    File.SetCreationTime(nuevaRuta, fechaFalsa);
                    File.SetLastWriteTime(nuevaRuta, fechaFalsa);
                    File.SetLastAccessTime(nuevaRuta, fechaFalsa);
                }

                // Limpieza de Atributos, Propiedad y Equipo
                File.SetAttributes(nuevaRuta, FileAttributes.Normal);
                try
                {
                    var seguridad = esCarpetaActual ? (FileSystemSecurity)Directory.GetAccessControl(nuevaRuta) : File.GetAccessControl(nuevaRuta);
                    var adminGroup = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, null);
                    seguridad.SetOwner(adminGroup);

                    if (esCarpetaActual) Directory.SetAccessControl(nuevaRuta, (DirectorySecurity)seguridad);
                    else File.SetAccessControl(nuevaRuta, (FileSecurity)seguridad);
                }
                catch { } // Ignoramos si el PC tiene las ACL bloqueadas en este archivo
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aniquilar {rutaActual}: {ex.Message}");
            }
        }

    }
}