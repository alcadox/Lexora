using Lexora.Core; // Importamos nuestra arquitectura
using System;
using System.Windows.Forms;

namespace Lexora.Pantallas.InicioSesion
{
    public partial class RegistrarCuenta : Form
    {
        public RegistrarCuenta()
        {
            InitializeComponent();
        }

        private void lblIniciarSesion_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnCrearCuenta_Click(object sender, EventArgs e)
        {
            string nombreUsuario = textBoxNombreUsuario.Text.Trim();
            string email = textBoxemailUser.Text.Trim();
            string password = textBoxContrasena.Text;

            // Validación básica
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nombreUsuario))
            {
                MessageBox.Show("Debes rellenar todos los campos.", "Campos obligatorios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RegistrarUsuario(nombreUsuario, email, password);
        }

        private void RegistrarUsuario(string nombre, string email, string password)
        {
            try
            {
                // 1. Verificamos en BD si el correo ya existe ANTES de mandar correos
                if (GestorDBAuth.ExisteEmail(email))
                {
                    MessageBox.Show("Ese correo electrónico ya está registrado.", "Aviso Lexora", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Generar Token
                string token = Guid.NewGuid().ToString().Substring(0, 7).ToUpper();

                // 3. Enviar correo usando nuestra utilidad modular
                CorreoUtil.EnviarTokenRegistro(email, token);

                // 4. Abrir verificación
                FormVerificacion formularioVerificar = new FormVerificacion(email, token);
                if (formularioVerificar.ShowDialog() == DialogResult.OK)
                {
                    // 5. Cifrado y guardado
                    string passwordHash = SeguridadUtil.CalcularHashSHA256(password);

                    if (GestorDBAuth.RegistrarNuevoUsuario(nombre, email, passwordHash))
                    {
                        // CORRECCIÓN: Auto-Login inmediato tras registro exitoso
                        var datosUsuario = GestorDBAuth.ObtenerDatosUsuario(email);
                        if (datosUsuario.Existe)
                        {
                            string tokenGenerado = GestorDBAuth.RegistrarLoginExitoso(datosUsuario.IdUsuario, email);

                            // Guardamos en Settings de la aplicación para que el MainForm lo reciba activo
                            Properties.Settings.Default.UsuarioRecordado = datosUsuario.Nombre;
                            Properties.Settings.Default.TokenSesion = tokenGenerado;
                            Properties.Settings.Default.Save();
                        }

                        MessageBox.Show("Cuenta verificada y creada con éxito. Entrando a Lexora...", "Registro completado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ocurrió un error inesperado al guardar la cuenta.", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Registro cancelado. La cuenta no se ha creado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error en el proceso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}