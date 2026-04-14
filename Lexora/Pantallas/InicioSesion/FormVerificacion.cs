using System;
using System.Windows.Forms;

namespace Lexora.Pantallas.InicioSesion
{
    public partial class FormVerificacion : Form
    {
        private string email;
        private string token;

        public FormVerificacion(string email, string token)
        {
            InitializeComponent();
            this.email = email;
            this.token = token;
        }

        private void buttonValidarCodigo_Click(object sender, EventArgs e)
        {
            string codigoIngresado = txtCodigoUsuario.Text.Trim().ToUpper();

            if (codigoIngresado == token)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("El código es incorrecto. Por favor, revísalo e inténtalo de nuevo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}