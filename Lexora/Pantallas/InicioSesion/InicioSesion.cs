using Npgsql;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Lexora.Pantallas.InicioSesion;



namespace Lexora
{
    public partial class InicioSesion : Form
    {
        private bool desdeMainForm = false;

        //para que MainForm sepa el nombre del usuario
        public string NombreUsuario { get; private set; }


        public InicioSesion()
        {
            InitializeComponent();
            
        }
        public InicioSesion(bool desdeMainForm)
        {
            InitializeComponent();
            this.desdeMainForm = desdeMainForm;
        }

        private void lblPWolvidado_Click(object sender, EventArgs e)
        {

        }

        private void btnOmitir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            if (this.desdeMainForm)
            {
                this.Close();
                return;
            }

            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
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

            LoginUsuario(email, password);
        }

        private void LoginUsuario(string email, string password)
        {
            string passwordHash = CalcularHashSHA256(password);
            string connectionString = ConfigurationManager
                .ConnectionStrings["conexionDBLexora"]
                .ConnectionString;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"SELECT nombre, contrasena_hash, activo
                           FROM usuario
                           WHERE email = @email;";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("No existe ningún usuario con ese e-mail.", "Login",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            bool activo = reader.GetBoolean(reader.GetOrdinal("activo"));
                            string hashDb = reader.GetString(reader.GetOrdinal("contrasena_hash"));
                            NombreUsuario = reader.GetString(reader.GetOrdinal("nombre"));

                            if (!activo)
                            {
                                MessageBox.Show("El usuario está desactivado.", "Login",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            if (!string.Equals(hashDb, passwordHash, StringComparison.Ordinal))
                            {
                                MessageBox.Show("Contraseña incorrecta.", "Login",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }



                            //LOGIN CORRECTO
                            this.DialogResult = DialogResult.OK;

                            if (desdeMainForm) // si se abrió desde MainForm
                            {
                                this.Close();
                            }
                            else // si se abrió de forma independiente (al inicio)
                            {
                                MainForm mainForm = new MainForm(NombreUsuario);
                                mainForm.Show();
                            }
                            this.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("connect"))
                {
                    MessageBox.Show("No se ha podido conectar con la base de datos. " +
                        "Comprueba tu conexión a Internet.", "Error de conexión",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Error al validar el usuario: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void lblCrearCuenta_Click(object sender, EventArgs e)
        {
            // Abrir formulario de registro
            RegistrarCuenta registrarCuentaForm = new RegistrarCuenta();

            // Ocultar el formulario de inicio de sesión mientras se muestra el de registro
            Hide();

            // Mostrar el formulario de registro como un cuadro de diálogo modal
            var result = registrarCuentaForm.ShowDialog();

            // Después de cerrar el formulario de registro, mostrar nuevamente el formulario de inicio de sesión
            if (result == DialogResult.OK || result == DialogResult.Cancel)
            {
                Show();
            }

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
