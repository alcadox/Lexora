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
            panelInformacion.BackColor = colorFondo;
            panelInformacion.Size = new Size(364, alto);
            labelTextoInformacion.Text = mensaje;
            panelInformacion.Visible = true;
        }

        private void lblPWolvidado_Click(object sender, EventArgs e)
        {
            string email = emailUser.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MostrarMensaje("Debes introducir tu e-mail para recuperar la contraseña...", colorPanelRojo, 95);
                lblEmail.ForeColor = Color.Red; emailUser.BackColor = colorPanelRojo; emailUser.Focus();
                return;
            }

            lblEmail.ForeColor = colorOriginalLetras; emailUser.BackColor = Color.White; panelInformacion.Visible = false;

            try
            {
                // USAMOS NUESTRO NUEVO GESTOR MODULAR
                var datosUsuario = GestorDBAuth.ObtenerDatosUsuario(email);

                if (!datosUsuario.Existe) { MostrarMensaje("No existe ningún usuario con ese e-mail.", colorPanelRojo); return; }
                if (!datosUsuario.Activo) { MostrarMensaje("El usuario está desactivado.", colorPanelRojo); return; }

                NombreUsuario = datosUsuario.Nombre;
                string token = Guid.NewGuid().ToString().Substring(0, 7).ToUpper();

                // USAMOS NUESTRO NUEVO SERVICIO DE CORREO
                CorreoUtil.EnviarTokenRecuperacion(email, token);

                FormVerificacion formularioVerificar = new FormVerificacion(email, token);
                if (formularioVerificar.ShowDialog() == DialogResult.OK)
                {
                    lblContrasena.Text = "Nueva Contraseña";
                    MostrarMensaje("Ya puedes introducir tu nueva contraseña...", colorPanelAmarillo, 110);
                    pwUser.Focus(); pwUser.BackColor = colorPanelAmarillo;
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

            emailUser.BackColor = string.IsNullOrEmpty(email) ? colorPanelRojo : Color.White;
            pwUser.BackColor = string.IsNullOrEmpty(password) ? colorPanelRojo : Color.White;

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

                // USAMOS NUESTRO NUEVO GESTOR CRIPTOGRÁFICO
                string passwordHash = SeguridadUtil.CalcularHashSHA256(password);

                if (datosUsuario.HashBD != passwordHash) { MostrarMensaje("Contraseña incorrecta.", colorPanelRojo); return; }

                NombreUsuario = datosUsuario.Nombre;
                this.DialogResult = DialogResult.OK;

                if (desdeMainForm) this.Close();
                else { new MainForm(NombreUsuario).Show(); }
                this.Hide();
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
                    MostrarMensaje("Contraseña actualizada correctamente.", Color.LightGreen);
                    this.DialogResult = DialogResult.OK;
                    cambioContrasena = false;

                    if (desdeMainForm) this.Close();
                    else { new MainForm(NombreUsuario).Show(); }
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al actualizar: {ex.Message}", colorPanelRojo, 97);
            }
        }

        private void btnOmitir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            if (desdeMainForm) { this.Close(); return; }
            new MainForm().Show(); this.Hide();
        }

        private void lblCrearCuenta_Click(object sender, EventArgs e)
        {
            Hide();
            var result = new RegistrarCuenta().ShowDialog();
            if (result == DialogResult.OK || result == DialogResult.Cancel) Show();
        }
    }
}