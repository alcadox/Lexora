using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lexora.Pantallas.InicioSesion
{
    public partial class FormVerificacion : Form
    {
        string email;
        string token;

        
        public FormVerificacion(string email, string token)
        {
            InitializeComponent();
            this.email = email;
            this.token = token;
        }

        private void buttonValidarCodigo_Click(object sender, EventArgs e)
        {

            string codigoIngresado = txtCodigoUsuario.Text.Trim().ToUpper();
            string emailUsuario = email; 

            if (codigoIngresado == token) { 

                DialogResult = DialogResult.OK; // Cerramos el form diciendo que todo fue bien
                Close();
            }
            else
            {
                MessageBox.Show("El código es incorrecto.");
            }
        }

    }
}
