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
                                    MessageBox.Show("No existe ningún usuario con ese e-mail.", "Login",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                // leer datos del usuario
                                bool activo = reader.GetBoolean(reader.GetOrdinal("activo"));
                                string hashDb = reader.GetString(reader.GetOrdinal("contrasena_hash"));
                                NombreUsuario = reader.GetString(reader.GetOrdinal("nombre"));

                                if (!activo)
                                {
                                    MessageBox.Show("El usuario está desactivado.", "Login",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                    MessageBox.Show("No se pudo enviar el correo de verificación. Inténtalo de nuevo.\nError: " + ex.Message,
                                                    "Error de envío", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return; // Salimos
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
        }

        private void ComprobarCorreoElectronico(string email, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Lexora", "lexora.confirmacion@gmail.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Cambio de Contrasñea Lexora";

            var bodyBuilder = new BodyBuilder();


            bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial; padding: 20px; border: 1px solid #ddd;'>
                <h1>Verifica tu correo</h1>
                <p>Para completar tu cambio de contraseña, introduce el siguiente código en la aplicación:</p>
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

        private void btnOmitir_Click(object sender, EventArgs e)
        {
            // cerrar el formulario de inicio de sesión y volver a MainForm
            this.DialogResult = DialogResult.Cancel;

            // si se abrió desde MainForm, simplemente cerrar
            if (this.desdeMainForm)
            {
                this.Close();
                return;
            }
            // si se abrió de forma independiente, abrir MainForm
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

            // 3.Intentar iniciar sesión o cambiar la contraseña
            if (cambioContrasena) ActualizarContrasena(email, password);
            else LoginUsuario(email, password);
                
        }

        private void ActualizarContrasena(string email, string nuevaContrasena)
        {
            // actualizar la contraseña en la base de datos
            string nuevaContrasenaHash = CalcularHashSHA256(nuevaContrasena);
            string connectionString = ConfigurationManager
                .ConnectionStrings["conexionDBLexora"]
                .ConnectionString;
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

                        // verificar si se actualizó alguna fila
                        if (filasAfectadas == 0)
                        {
                            MessageBox.Show("No se pudo actualizar la contraseña. Inténtalo de nuevo.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show("Contraseña actualizada correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // una vez cambiada la contraseña, iniciar sesión automáticamente
                            this.DialogResult = DialogResult.OK;
                            cambioContrasena = false;

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
                MessageBox.Show("Error al actualizar la contraseña: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginUsuario(string email, string password)
        {
            // validar el usuario en la base de datos
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
