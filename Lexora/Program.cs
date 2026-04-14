using System;
using System.Configuration;
using System.Windows.Forms;

namespace Lexora
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. BLINDAJE DE CONEXIÓN
            ProtegerConfiguracion();

            // 2. COMPROBACIÓN DE SESIÓN MAESTRA
            string tokenGuardado = Properties.Settings.Default.TokenSesion;
            bool sesionActiva = false;

            if (!string.IsNullOrEmpty(tokenGuardado))
            {
                var usuario = Lexora.Core.GestorDBAuth.ValidarTokenSesion(tokenGuardado);
                if (usuario.Existe && usuario.Activo)
                {
                    sesionActiva = true;
                }
            }

            // 3. ENRUTAMIENTO LIMPIO SIN VENTANAS FANTASMA
            if (sesionActiva)
            {
                // Entramos directo, MainForm leerá la sesión
                Application.Run(new MainForm());
            }
            else
            {
                // Mostramos el login como cuadro de diálogo bloqueante
                InicioSesion ventanaLogin = new InicioSesion(false);
                DialogResult resultado = ventanaLogin.ShowDialog();

                // Si se loguea (OK) o le da a Omitir (Ignore), abrimos el programa. 
                // Si cierra en la X, la app simplemente termina, ahorrando memoria.
                if (resultado == DialogResult.OK || resultado == DialogResult.Ignore)
                {
                    Application.Run(new MainForm());
                }
            }
        }

        private static void ProtegerConfiguracion()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConfigurationSection section = config.GetSection("connectionStrings");
                if (section != null && !section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    section.SectionInformation.ForceSave = true;
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Advertencia de Seguridad: " + ex.Message);
            }
        }
    }
}