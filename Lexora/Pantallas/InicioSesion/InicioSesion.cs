using Lexora.Pantallas.InicioSesion;
using MailKit.Security;
using MimeKit;
using Npgsql;
using System;
using System.Configuration;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MailKit.Net.Smtp;

namespace Lexora
{
    public partial class InicioSesion : Form
    {
        // colores personalizados
        Color colorPanelInformacionRojo = Color.FromArgb(255, 161, 161);
        Color colorPanelInformacionAmarillo = Color.FromArgb(236, 255, 161);
        Color colorOriginalLetrasLabels = Color.FromArgb(64, 0, 64);

        // para saber si se abrió desde MainForm
        private bool desdeMainForm = false;

        // para saber si se va a cambiar la contraseña
        private bool cambioContrasena = false;

        // para que MainForm sepa el nombre del usuario
        public string NombreUsuario { get; private set; }

        // constructor por defecto
        public InicioSesion()
        {
            InitializeComponent();
        }

        // constructor sobrecargado para saber si se abrió desde MainForm
        public InicioSesion(bool desdeMainForm)
        {
            InitializeComponent();
            this.desdeMainForm = desdeMainForm;
        }

        // botón olvidé mi contraseña
        private void lblPWolvidado_Click(object sender, EventArgs e)
        {
            // validar que el campo de e-mail no esté vacío
            if (string.IsNullOrEmpty(emailUser.Text.Trim()))
            {
                // Mostrar mensaje de error si el campo de e-mail está vacío
                panelInformacion.BackColor = colorPanelInformacionRojo;
                panelInformacion.Size = new Size(364, 95);
                labelTextoInformacion.Text = "Debes introducir tu e-mail para recuperar la contraseña" + Environment.NewLine + "y pulsar nueva mente '¿Has olvidado tu contraseña?'";
                panelInformacion.Visible = true;

                lblEmail.ForeColor = Color.Red;

                emailUser.Focus();
                emailUser.BackColor = colorPanelInformacionRojo;

                return;
            }
            else // el campo de e-mail está relleno
            {   // restaurar colores originales
                lblEmail.ForeColor = colorOriginalLetrasLabels;
                emailUser.BackColor = Color.White;
                panelInformacion.Visible = false;

                // comprobar si el e-mail existe en la base de datos
                try
                {
                    // obtener cadena de conexión
                    string connectionString = ConfigurationManager.ConnectionStrings["conexionDBLexora"].ConnectionString;
                    string email = emailUser.Text.Trim();

                    // conectar a la base de datos
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();

                        // comprobar si el e-mail existe
                        string sql = @"SELECT nombre, contrasena_hash, activo
                           FROM usuario
                           WHERE email = @email;";

                        using (var cmd = new NpgsqlCommand(sql, conn))
                        {
                            // añadir parámetro
                            cmd.Parameters.AddWithValue("@email", email);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    panelInformacion.BackColor = colorPanelInformacionRojo;
                                    panelInformacion.Visible = true;
                                    panelInformacion.Size = new Size(364, 75);
                                    labelTextoInformacion.Text = "No existe ningún usuario con ese e-mail.";
                                    return;
                                }

                                // leer datos del usuario
                                bool activo = reader.GetBoolean(reader.GetOrdinal("activo"));
                                string hashDb = reader.GetString(reader.GetOrdinal("contrasena_hash"));
                                NombreUsuario = reader.GetString(reader.GetOrdinal("nombre"));

                                if (!activo)
                                {
                                    panelInformacion.BackColor = colorPanelInformacionRojo;
                                    panelInformacion.Visible = true;
                                    panelInformacion.Size = new Size(364, 75);
                                    labelTextoInformacion.Text = "El usuario está desactivado.";
                                    return;
                                }
                                string token = Guid.NewGuid().ToString().Substring(0, 7).ToUpper();

                                // enviar e-mail de recuperación de contraseña
                                try
                                {
                                    ComprobarCorreoElectronico(email, token);
                                }
                                catch (Exception ex)
                                {
                                    panelInformacion.BackColor = colorPanelInformacionRojo;
                                    panelInformacion.Visible = true;
                                    panelInformacion.Size = new Size(364, 97);
                                    labelTextoInformacion.Text = "No se pudo enviar el correo de verificación: " + Environment.NewLine + ex.Message;
                                    return;
                                }

                                // mostrar formulario para que el usuario introduzca el código recibido por e-mail
                                FormVerificacion formularioVerificar = new FormVerificacion(email, token);
                                var resultado = formularioVerificar.ShowDialog();

                                // si el código es correcto, permitir cambiar la contraseña
                                if (resultado == DialogResult.OK)
                                {
                                    lblContrasena.Text = "Nueva Contraseña";

                                    // informar al usuario de que ya puede cambiar la contraseña
                                    panelInformacion.BackColor = colorPanelInformacionAmarillo;
                                    panelInformacion.Size = new Size(364, 110);
                                    labelTextoInformacion.Text = "Ya puedes introducir tu nueva contraseña, una vez" + Environment.NewLine + "introducida e iniciada la sesión se cambiará " + Environment.NewLine + "automáticamente.";
                                    panelInformacion.Visible = true;

                                    pwUser.Focus();
                                    pwUser.BackColor = colorPanelInformacionAmarillo;

                                    // marcar que se va a cambiar la contraseña
                                    cambioContrasena = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    panelInformacion.BackColor = colorPanelInformacionRojo;
                    panelInformacion.Visible = true;
                    if (ex.Message.Contains("connect"))
                    {
                        panelInformacion.Size = new Size(364, 100);
                        labelTextoInformacion.Text = "No se ha podido conectar con la base de datos." + Environment.NewLine + Environment.NewLine + "Comprueba tu conexión a Internet.";
                    }
                    else
                    {
                        panelInformacion.Size = new Size(364, 75);
                        labelTextoInformacion.Text = "Error al validar el usuario: " + ex.Message;
                    }
                }
            }
        }

        private void ComprobarCorreoElectronico(string email, string token)
        {
            // 1. Leer las credenciales de forma segura desde la configuración
            string emisorCorreo = ConfigurationManager.AppSettings["CorreoEmisor"];
            string emisorPassword = ConfigurationManager.AppSettings["PasswordEmisor"];

            if (string.IsNullOrEmpty(emisorCorreo) || string.IsNullOrEmpty(emisorPassword))
            {
                throw new Exception("Faltan las credenciales del servidor de correo. Configura 'CredencialesCorreo.config'.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Lexora", emisorCorreo));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Cambio de Contraseña Lexora";

            var bodyBuilder = new BodyBuilder();

            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial; padding: 20px; border: 1px solid #ddd; border-radius: 8px; max-width: 500px;'>
                    <h1 style='color: #2c3e50;'>Verifica tu correo</h1>
                    <p>Para completar tu cambio de contraseña, introduce el siguiente código en la aplicación:</p>
                    <div style='background-color: #f8f9fa; padding: 15px; text-align: center; border-radius: 5px; margin: 20px 0;'>
                        <h2 style='color: #e74c3c; letter-spacing: 5px; margin: 0;'>{token}</h2>
                    </div>
                    <p style='color: #7f8c8d; font-size: 12px;'>Si no has solicitado este código, ignora este mensaje o contacta con el soporte de Lexora.</p>
                </div>";

            message.Body = bodyBuilder.ToMessageBody();

            // 2. Uso de 'using' para asegurar la liberación de recursos (Optimización de red)
            using (var client = new SmtpClient())
            {
                // El timeout evita que la app se quede colgada si no hay red
                client.Timeout = 10000;

                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // Autenticación usando las variables seguras
                client.Authenticate(emisorCorreo, emisorPassword);

                client.Send(message);
                client.Disconnect(true);
            }
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
            string email = emailUser.Text.Trim();
            string password = pwUser.Text;

            if (string.IsNullOrEmpty(email)) emailUser.BackColor = colorPanelInformacionRojo;
            else emailUser.BackColor = Color.White;

            if (string.IsNullOrEmpty(password)) pwUser.BackColor = colorPanelInformacionRojo;
            else pwUser.BackColor = Color.White;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                panelInformacion.BackColor = colorPanelInformacionRojo;
                panelInformacion.Visible = true;
                panelInformacion.Size = new Size(364, 75);
                labelTextoInformacion.Text = "Debes rellenar el e-mail y la contraseña.";
                

                return;
            }

            if (cambioContrasena) ActualizarContrasena(email, password);
            else LoginUsuario(email, password);
        }

        private void ActualizarContrasena(string email, string nuevaContrasena)
        {
            string nuevaContrasenaHash = CalcularHashSHA256(nuevaContrasena);
            string connectionString = ConfigurationManager.ConnectionStrings["conexionDBLexora"].ConnectionString;
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"UPDATE usuario
                                   SET contrasena_hash = @contrasena_hash
                                   WHERE email = @email;";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@contrasena_hash", nuevaContrasenaHash);
                        cmd.Parameters.AddWithValue("@email", email);
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            panelInformacion.BackColor = colorPanelInformacionRojo;
                            panelInformacion.Visible = true;
                            panelInformacion.Size = new Size(364, 95);
                            labelTextoInformacion.Text = "No se pudo actualizar la contraseña. " + Environment.NewLine +"Inténtalo de nuevo.";
                        }
                        else
                        {
                            // Éxito: En este caso sí mantenemos un aviso positivo antes de redirigir
                            panelInformacion.BackColor = Color.LightGreen;
                            panelInformacion.Visible = true;
                            panelInformacion.Size = new Size(364, 75);
                            labelTextoInformacion.Text = "Contraseña actualizada correctamente.";

                            this.DialogResult = DialogResult.OK;
                            cambioContrasena = false;

                            if (desdeMainForm)
                            {
                                this.Close();
                            }
                            else
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
                panelInformacion.BackColor = colorPanelInformacionRojo;
                panelInformacion.Visible = true;
                panelInformacion.Size = new Size(364, 97);
                labelTextoInformacion.Text = "Error al actualizar la contraseña: " + Environment.NewLine + ex.Message;
            }
        }

        private void LoginUsuario(string email, string password)
        {
            string passwordHash = CalcularHashSHA256(password);
            string connectionString = ConfigurationManager.ConnectionStrings["conexionDBLexora"].ConnectionString;

            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT nombre, contrasena_hash, activo FROM usuario WHERE email = @email;";

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                panelInformacion.BackColor = colorPanelInformacionRojo;
                                panelInformacion.Visible = true;
                                panelInformacion.Size = new Size(364, 75);
                                labelTextoInformacion.Text = "No existe ningún usuario con ese e-mail.";
                                return;
                            }

                            bool activo = reader.GetBoolean(reader.GetOrdinal("activo"));
                            string hashDb = reader.GetString(reader.GetOrdinal("contrasena_hash"));
                            NombreUsuario = reader.GetString(reader.GetOrdinal("nombre"));

                            if (!activo)
                            {
                                panelInformacion.BackColor = colorPanelInformacionRojo;
                                panelInformacion.Visible = true;
                                panelInformacion.Size = new Size(364, 75);
                                labelTextoInformacion.Text = "El usuario está desactivado.";
                                return;
                            }

                            if (!string.Equals(hashDb, passwordHash, StringComparison.Ordinal))
                            {
                                panelInformacion.BackColor = colorPanelInformacionRojo;
                                panelInformacion.Visible = true;
                                panelInformacion.Size = new Size(364, 75);
                                labelTextoInformacion.Text = "Contraseña incorrecta.";
                                return;
                            }

                            this.DialogResult = DialogResult.OK;
                            if (desdeMainForm) this.Close();
                            else
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
                panelInformacion.BackColor = colorPanelInformacionRojo;
                panelInformacion.Visible = true;
                if (ex.Message.Contains("connect"))
                {
                    panelInformacion.Size = new Size(364, 100);
                    labelTextoInformacion.Text = "No se ha podido conectar con la base de datos." + Environment.NewLine + Environment.NewLine + "Comprueba tu conexión a Internet.";
                }
                else
                {
                    panelInformacion.Size = new Size(364, 96);
                    labelTextoInformacion.Text = "Error al validar el usuario: " + Environment.NewLine + ex.Message;
                }
            }
        }

        private void lblCrearCuenta_Click(object sender, EventArgs e)
        {
            RegistrarCuenta registrarCuentaForm = new RegistrarCuenta();
            Hide();
            var result = registrarCuentaForm.ShowDialog();
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
                foreach (byte b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}