using MailKit.Security;
using MimeKit;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MailKit.Net.Smtp;

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
            //1.Leo valores de los TextBox
            string nombreUsuario = textBoxNombreUsuario.Text;
            string email = textBoxemailUser.Text.Trim();
            string password = textBoxContrasena.Text;
            string confirmarPassword = textBoxContrasena.Text;


            //2.Validación básica
            if (
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(confirmarPassword) || string.IsNullOrEmpty(nombreUsuario)
                )
            {
                MessageBox.Show("Debes rellenar todos los campos.", "Campos obligatorios",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            } else if (!password.Equals(confirmarPassword))
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error de validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            RegistrarUsuario(nombreUsuario, email, password);
        }

        private void RegistrarUsuario(string nombre, string email, string password)
        {
            // 1. Preparar los datos
            string passwordHash = CalcularHashSHA256(password);

            // Generar Token
            string token = Guid.NewGuid().ToString().Substring(0, 7).ToUpper();

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
                DialogResult = DialogResult.Cancel;
            }

            // 5. Cerrar el formulario de registro
            DialogResult = DialogResult.OK;
            this.Close();
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

    }
}
