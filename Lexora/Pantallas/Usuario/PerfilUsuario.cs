using Lexora.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lexora.Pantallas.Usuario
{
    public partial class PerfilUsuario : Form
    {
        private DatosUsuarioAuth usuario;

        public PerfilUsuario(DatosUsuarioAuth datosUsuario)
        {
            InitializeComponent();
            this.usuario = datosUsuario;
            CargarDatos();
        }

        private void CargarDatos()
        {
            txtNombre.Text = usuario.Nombre;
            txtEmail.Text = usuario.Email;
            lblPuntosIA.Text = $"Puntos IA Restantes: {usuario.PuntosIA}";
        }

        // BOTÓN GUARDAR CAMBIOS
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nuevoNombre = txtNombre.Text.Trim();
            string nuevoEmail = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrEmpty(nuevoEmail))
            {
                MessageBox.Show("Los campos no pueden estar vacíos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (GestorDBAuth.ActualizarPerfil(usuario.IdUsuario, nuevoNombre, nuevoEmail))
                {
                    MessageBox.Show("Perfil actualizado con éxito.", "Lexora", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; // Le dice al MainForm que refresque
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error al actualizar la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // BOTÓN CERRAR SESIÓN
        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort; // Usamos Abort como señal de Cerrar Sesión para el MainForm
            this.Close();
        }

        // BOTÓN CAMBIAR DE CUENTA
        private void btnCambiarCuenta_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore; // Usamos Ignore como señal de Cambiar Cuenta
            this.Close();
        }

        // BOTÓN CAMBIAR CONTRASEÑA
        private void btnCambiarPassword_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Se ha enviado un código de seguridad a tu correo para cambiar la contraseña.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reutilizamos tu arquitectura ya creada:
            string token = Guid.NewGuid().ToString().Substring(0, 7).ToUpper();
            CorreoUtil.EnviarTokenRecuperacion(usuario.Email, token);

            using (Lexora.Pantallas.InicioSesion.FormVerificacion frm = new Lexora.Pantallas.InicioSesion.FormVerificacion(usuario.Email, token))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Contraseña actualizada. Por seguridad, vuelve a iniciar sesión.", "Lexora", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.Abort; // Forzamos cierre de sesión por seguridad tras cambiar la password
                    this.Close();
                }
            }
        }

        // BOTÓN CERRAR / CANCELAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}