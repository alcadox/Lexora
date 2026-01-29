using MailKit.Security;
using MimeKit;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit.Net.Smtp;
using Lexora.Pantallas.InicioSesion;



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
            // 1. Preparar los datos
            string nombre = email; // Usar el email como nombre 
            string passwordHash = CalcularHashSHA256(password);

            // Generar Token
            string token = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            // 2. Intentar enviar el correo primero (para no guardar nada si falla el envío)
            try
            {
                ComprobarCorreoElectronico(email, token);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo enviar el correo de verificación. Inténtalo de nuevo.\nError: " + ex.Message,
                                "Error de envío", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Salimos, no abrimos el formulario ni guardamos en BD
            }

            // 3. Abrir cuadro de diálogo para verificar
            // Pasamos el token al otro formulario para que compare
            FormVerificacion formularioVerificar = new FormVerificacion(email, token);
            var resultado = formularioVerificar.ShowDialog();

            // 4. Si el usuario puso el código bien (DialogResult.OK) -> GUARDAMOS EN BD
            if (resultado == DialogResult.OK)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexionDBLexora"].ConnectionString;

                try
                {
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();

                        // SQL: No insertamos 'id_usuario' (es SERIAL) ni 'fecha_registro' (es DEFAULT NOW)
                        // Insertamos 'activo' como TRUE directamente porque ya pasó la verificación
                        string sql = @"INSERT INTO usuario (nombre, email, contrasena_hash, activo)
                               VALUES (@nombre, @email, @hash, @activo);";

                        using (var cmd = new NpgsqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@nombre", nombre);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@hash", passwordHash);
                            cmd.Parameters.AddWithValue("@activo", true); // <--- IMPORTANTE: True

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Cuenta verificada y creada con éxito. Ya puedes iniciar sesión.",
                                    "Registro completado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reseteamos la interfaz para volver al modo Login
                    modoRegistro = false;
                    btnIniciarSesion.Text = "Iniciar sesión";
                    lblEmail.Text = "E-mail";
                    lblContrasena.Text = "Contraseña";

                    // Limpiamos los campos para que escriba sus datos de nuevo al loguearse
                    emailUser.Text = "";
                    pwUser.Text = "";
                }
                catch (PostgresException ex) when (ex.SqlState == "23505") // Código de error para UNIQUE VIOLATION
                {
                    MessageBox.Show("Ese correo electrónico ya está registrado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ocurrió un error al guardar en la base de datos: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Si cerró la ventana o falló la validación
                MessageBox.Show("Registro cancelado. La cuenta no se ha creado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ComprobarCorreoElectronico(string email, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Lexora", "lexora.confirmacion@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verifica tu cuenta de Lexora";

            var bodyBuilder = new BodyBuilder(); 


            bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial; padding: 20px; border: 1px solid #ddd;'>
                <h1>Bienvenido a Lexora</h1>
                <p>Para completar tu registro, introduce el siguiente código en la aplicación:</p>
                <h2 style='color: #2c3e50; letter-spacing: 5px;'>{token}</h2>
                <p>Si no has solicitado este código, ignora este mensaje.</p>
            </div>";

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("lexora.confirmacion@gmail.com", "edlq nuou lbkc gajd");
                client.Send(message);
                client.Disconnect(true);
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
