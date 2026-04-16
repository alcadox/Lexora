using Lexora.Core;
using Lexora.Pantallas.InicioSesion;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lexora
{
    public partial class InicioSesion : Form
    {
        Color colorPanelRojo = Color.FromArgb(255, 161, 161);
        Color colorPanelAmarillo = Color.FromArgb(236, 255, 161);
        Color colorOriginalLetras = Color.FromArgb(64, 0, 64);

        private bool desdeMainForm = false;
        private bool cambioContrasena = false;
        public string NombreUsuario { get; private set; }

        public InicioSesion() { InitializeComponent(); }
        public InicioSesion(bool desdeMainForm) { InitializeComponent(); this.desdeMainForm = desdeMainForm; }

        private void MostrarMensaje(string mensaje, Color colorFondo, int alto = 75)
        {
            panelInformacion.FillColor = colorFondo;
            panelInformacion.Size = new Size(364, alto);
            labelTextoInformacion.Text = mensaje;
            panelInformacion.Visible = true;
        }

        private void lblPWolvidado_Click(object sender, EventArgs e)
        {
            string email = emailUser.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MostrarMensaje("Debes introducir tu e-mail para recuperar la \ncontraseña...", colorPanelRojo, 95);
                lblEmail.ForeColor = Color.Red; emailUser.FillColor = colorPanelRojo; emailUser.Focus();
                return;
            }

            lblEmail.ForeColor = colorOriginalLetras; emailUser.FillColor = Color.White; panelInformacion.Visible = false;

            try
            {
                var datosUsuario = GestorDBAuth.ObtenerDatosUsuario(email);

                if (!datosUsuario.Existe) { MostrarMensaje("No existe ningún usuario con ese e-mail.", colorPanelRojo); return; }
                if (!datosUsuario.Activo) { MostrarMensaje("El usuario está desactivado.", colorPanelRojo); return; }

                NombreUsuario = datosUsuario.Nombre;
                string token = Guid.NewGuid().ToString().Substring(0, 7).ToUpper();

                CorreoUtil.EnviarTokenRecuperacion(email, token);

                FormVerificacion formularioVerificar = new FormVerificacion(email, token);
                if (formularioVerificar.ShowDialog() == DialogResult.OK)
                {
                    lblContrasena.Text = "Nueva Contraseña";
                    MostrarMensaje("Ya puedes introducir tu nueva contraseña...", colorPanelAmarillo, 110);
                    pwUser.Focus(); pwUser.FillColor = colorPanelAmarillo;
                    cambioContrasena = true;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error: {ex.Message}", colorPanelRojo, 95);
            }
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            string email = emailUser.Text.Trim();
            string password = pwUser.Text;

            emailUser.FillColor = string.IsNullOrEmpty(email) ? colorPanelRojo : Color.White;
            pwUser.FillColor = string.IsNullOrEmpty(password) ? colorPanelRojo : Color.White;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MostrarMensaje("Debes rellenar el e-mail y la contraseña.", colorPanelRojo);
                return;
            }

            if (cambioContrasena) ActualizarContrasena(email, password);
            else LoginUsuario(email, password);
        }

        private void LoginUsuario(string email, string password)
        {
            try
            {
                var datosUsuario = GestorDBAuth.ObtenerDatosUsuario(email);

                if (!datosUsuario.Existe) { MostrarMensaje("No existe ningún usuario con ese e-mail.", colorPanelRojo); return; }
                if (!datosUsuario.Activo) { MostrarMensaje("El usuario está desactivado.", colorPanelRojo); return; }

                if (datosUsuario.BloqueadoHasta.HasValue && datosUsuario.BloqueadoHasta.Value > DateTime.Now)
                {
                    TimeSpan tiempoRestante = datosUsuario.BloqueadoHasta.Value - DateTime.Now;
                    MostrarMensaje($"Cuenta bloqueada por múltiples fallos. \nInténtalo en {tiempoRestante.Minutes} min y {tiempoRestante.Seconds} seg.", colorPanelRojo, 95);
                    return;
                }

                string passwordHash = SeguridadUtil.CalcularHashSHA256(password);

                if (datosUsuario.HashBD != passwordHash)
                {
                    int fallos = GestorDBAuth.RegistrarIntentoFallido(email, datosUsuario.IntentosFallidos);
                    if (fallos >= 5)
                        MostrarMensaje("Demasiados intentos fallidos. \nCuenta bloqueada 15 minutos.", colorPanelRojo, 95);
                    else
                        MostrarMensaje($"Contraseña incorrecta. (Intento {fallos} de 5)", colorPanelRojo);

                    return;
                }

                string tokenGenerado = GestorDBAuth.RegistrarLoginExitoso(datosUsuario.IdUsuario, email);

                GestorLogs.IdUsuarioActual = datosUsuario.IdUsuario;
                GestorLogs.Registrar("LOGIN", "Usuario inició sesión con éxito desde la pantalla principal.");

                Properties.Settings.Default.UsuarioRecordado = datosUsuario.Nombre;
                Properties.Settings.Default.TokenSesion = tokenGenerado;
                Properties.Settings.Default.Save();

                NombreUsuario = datosUsuario.Nombre;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error de conexión: {ex.Message}", colorPanelRojo, 96);
            }
        }

        private void ActualizarContrasena(string email, string nuevaContrasena)
        {
            try
            {
                string nuevaContrasenaHash = SeguridadUtil.CalcularHashSHA256(nuevaContrasena);
                bool exito = GestorDBAuth.ActualizarContrasena(email, nuevaContrasenaHash);

                if (!exito)
                {
                    MostrarMensaje("No se pudo actualizar la contraseña. Inténtalo de nuevo.", colorPanelRojo, 95);
                }
                else
                {
                    MostrarMensaje("Contraseña actualizada. Iniciando sesión de forma segura...", Color.LightGreen);
                    cambioContrasena = false;

                    // CORRECCIÓN: Iniciar sesión automáticamente tras recuperar la contraseña
                    var datosUsuario = GestorDBAuth.ObtenerDatosUsuario(email);
                    if (datosUsuario.Existe)
                    {
                        string tokenGenerado = GestorDBAuth.RegistrarLoginExitoso(datosUsuario.IdUsuario, email);

                        GestorLogs.IdUsuarioActual = datosUsuario.IdUsuario;

                        GestorLogs.Registrar("RESET_PASS", "Contraseña restablecida.");
                        GestorLogs.Registrar("LOGIN_RECUPERACION", "Sesión iniciada automáticamente tras restablecer contraseña.");

                        Properties.Settings.Default.UsuarioRecordado = datosUsuario.Nombre;
                        Properties.Settings.Default.TokenSesion = tokenGenerado;
                        Properties.Settings.Default.Save();
                        NombreUsuario = datosUsuario.Nombre;
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al actualizar: {ex.Message}", colorPanelRojo, 97);
            }
        }

        private void btnOmitir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
            this.Close();
        }

        // CORRECCIÓN: Evento asignado y funcional con manejo de recursos limpio (using)
        private void lblCrearCuenta_Click(object sender, EventArgs e)
        {
            this.Hide(); // Ocultamos el login temporalmente
            using (RegistrarCuenta formRegistro = new RegistrarCuenta())
            {
                if (formRegistro.ShowDialog() == DialogResult.OK)
                {
                    // Si el registro devuelve OK (es decir, ya generó el token), cerramos el login también con OK
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    // Si el usuario canceló el registro, volvemos a mostrar la pantalla de Login
                    this.Show();
                }
            }
        }
    }
}