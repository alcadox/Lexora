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
        //true = estamos creando cuenta; false = iniciando sesión
        private bool modoRegistro = false;

        //para que MainForm sepa si el login fue correcto y el nombre del usuario
        public bool LoginCorrecto { get; private set; } = false;
        public string NombreUsuario { get; private set; } = "";
        public InicioSesion()
        {
            InitializeComponent();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblPWolvidado_Click(object sender, EventArgs e)
        {

        }

        private void btnOmitir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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

            if (modoRegistro)
            {
                //estamos en modo CREAR CUENTA
                RegistrarUsuario(email, password);
            }
            else
            {
                //estamos en modo LOGIN
                LoginUsuario(email, password);
            }
        }
        private void RegistrarUsuario(string email, string password)
        {
            //Nombre: uso el propio email como nombre
            string nombre = email;
            string passwordHash = CalcularHashSHA256(password);

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

                //después de crear cuenta, volvemos al modo LOGIN
                modoRegistro = false;
                btnIniciarSesion.Text = "Iniciar sesión";
                lblEmail.Text = "E-mail";
                lblContrasena.Text = "Contraseña";
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
                            LoginCorrecto = true;

                            //nombre a mostrar: todo lo que hay antes de @
                            string nombreMostrado;
                            int pos = email.IndexOf('@');
                            if (pos > 0)
                                nombreMostrado = email.Substring(0, pos);
                            else
                                nombreMostrado = reader.GetString(reader.GetOrdinal("nombre"));

                            NombreUsuario = nombreMostrado;

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al validar el usuario: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void lblCrearCuenta_Click(object sender, EventArgs e)
        {

            lblEmail.Text = "Nuevo E-mail:";
            lblContrasena.Text = "Nueva Contraseña:";
            modoRegistro = true;
            btnIniciarSesion.Text = "Crear cuenta";

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
