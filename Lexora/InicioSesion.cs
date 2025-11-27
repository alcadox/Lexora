using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Lexora
{
    public partial class InicioSesion : Form
    {
        public InicioSesion()
        {
            InitializeComponent();
            conectarPrueba();
            
        }

        

        private void conectarPrueba()
        {

            string connectionString = ConfigurationManager.ConnectionStrings["conexionDBLexora"].ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                MessageBox.Show("Conexión exitosa a la base de datos.");
            }
        }
        

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblPWolvidado_Click(object sender, EventArgs e)
        {

        }

        private void btnOmitir_Click(object sender, EventArgs e)
        {

        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {

        }
    }
}
