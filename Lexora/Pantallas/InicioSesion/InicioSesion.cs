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
using System.Security.Cryptography;
using System.Text;
using Npgsql;




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

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open(); // aquí sí comprueba de verdad
                    MessageBox.Show("Conexión exitosa a la base de datos.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message);
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
            MainForm ventanaPrincipal = new MainForm();
            ventanaPrincipal.Show();
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            //1.Leo valores de los TextBox
            string email = emailUser.Text.Trim();
            string password = pwUser.Text;

            //2.Validación básica
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Debes rellenar el e-mail y la contraseña.", "Campos obligatorios",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Nombre:uso el propio email como nombre
            string nombre = email;

            //3. Calcular hash de la contraseña
            string passwordHash = CalcularHashSHA256(password);

            //4. Obtener cadena de conexión del app.config (NO se toca app.config)
            string connectionString = ConfigurationManager
                .ConnectionStrings["conexionDBLexora"]
                .ConnectionString;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"INSERT INTO usuario (nombre, email, contrasena_hash, activo)
                           VALUES (@nombre, @email, @hash, @activo);";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@hash", passwordHash);
                        cmd.Parameters.AddWithValue("@activo", true);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Usuario registrado correctamente en la base de datos.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // unique_violation
            {
                MessageBox.Show("Ya existe un usuario con ese e-mail.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el usuario: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void lblCrearCuenta_Click(object sender, EventArgs e)
        {
            lblEmail.Text = "Nuevo E-mail:";
            lblContrasena.Text = "Nueva Contraseña:";
            

        }
        private string CalcularHashSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(texto);
                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

    }
}
