using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Configuration;

namespace Lexora.Core
{
    public static class CorreoUtil
    {
        public static void EnviarTokenRecuperacion(string email, string token)
        {
            string emisorCorreo = ConfigurationManager.AppSettings["CorreoEmisor"];
            string emisorPassword = ConfigurationManager.AppSettings["PasswordEmisor"];

            if (string.IsNullOrEmpty(emisorCorreo) || string.IsNullOrEmpty(emisorPassword))
                throw new Exception("Faltan las credenciales del servidor de correo. Configura 'CredencialesCorreo.config'.");

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

            using (var client = new SmtpClient())
            {
                client.Timeout = 10000;
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(emisorCorreo, emisorPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }
        public static void EnviarTokenRegistro(string email, string token)
        {
            string emisorCorreo = ConfigurationManager.AppSettings["CorreoEmisor"];
            string emisorPassword = ConfigurationManager.AppSettings["PasswordEmisor"];

            if (string.IsNullOrEmpty(emisorCorreo) || string.IsNullOrEmpty(emisorPassword))
                throw new Exception("Faltan credenciales en CredencialesCorreo.config.");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Lexora", emisorCorreo));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Verifica tu cuenta de Lexora";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
            <div style='font-family: Arial; padding: 20px; border: 1px solid #ddd; border-radius: 8px; max-width: 500px;'>
                <h1 style='color: #2c3e50;'>Bienvenido a Lexora</h1>
                <p>Para completar tu registro, introduce el siguiente código en la aplicación:</p>
                <div style='background-color: #f8f9fa; padding: 15px; text-align: center; border-radius: 5px; margin: 20px 0;'>
                    <h2 style='color: #1abc9c; letter-spacing: 5px; margin: 0;'>{token}</h2>
                </div>
                <p style='color: #7f8c8d; font-size: 12px;'>Si no has solicitado este registro, ignora este mensaje.</p>
            </div>";

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Timeout = 10000;
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate(emisorCorreo, emisorPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}