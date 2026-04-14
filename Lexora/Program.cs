using System;
using System.Configuration;
using System.Windows.Forms;

namespace Lexora
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. BLINDAJE DE CONEXIÓN: Auto-Encriptar App.config en la máquina del cliente
            ProtegerConfiguracion();

            // 2. Lógica de Persistencia de Sesión
            string tokenGuardado = Properties.Settings.Default.TokenSesion;

            if (!string.IsNullOrEmpty(tokenGuardado))
            {
                // Intentamos validar el token contra la DB
                var usuario = Lexora.Core.GestorDBAuth.ValidarTokenSesion(tokenGuardado);

                if (usuario.Existe)
                {
                    // Sesión válida - Entramos directo al MainForm
                    Application.Run(new MainForm(usuario.Nombre));
                    return;
                }
            }

            // 3. Si no hay token o no es válido, flujo normal
            Application.Run(new InicioSesion());
        }

        // --- SISTEMA DE PROTECCIÓN DPAPI ---
        private static void ProtegerConfiguracion()
        {
            try
            {
                // Abre el archivo de configuración del ejecutable actual (.exe.config)
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Seleccionamos la sección de las cadenas de conexión
                ConfigurationSection section = config.GetSection("connectionStrings");

                // Si la sección existe y NO está protegida, la encriptamos
                if (section != null && !section.SectionInformation.IsProtected)
                {
                    // Encripta la sección usando DPAPI (Data Protection API de Windows ligada a esta máquina/usuario)
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");

                    // Obliga a guardar los cambios cifrados en el disco duro
                    section.SectionInformation.ForceSave = true;
                    config.Save(ConfigurationSaveMode.Modified);

                    // Refresca la configuración en memoria para que Lexora pueda usarla encriptada
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
            }
            catch (Exception ex)
            {
                // En un entorno de producción, registraríamos esto en el Log. 
                // Por ahora, lo silenciamos para no interrumpir el arranque si hay un problema de permisos locales.
                Console.WriteLine("Advertencia de Seguridad Lexora: No se pudo cifrar la configuración. " + ex.Message);
            }
        }
    }
}