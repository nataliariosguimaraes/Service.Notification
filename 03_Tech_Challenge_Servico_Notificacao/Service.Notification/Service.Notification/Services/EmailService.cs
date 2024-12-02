using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Service.Notification.Services.Contracts;

namespace Service.Notification.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string name, string email, string body)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Tech Challenge", "MS_muqbtG@trial-neqvygm8vw540p7w.mlsender.net"));
        message.To.Add(new MailboxAddress(name, email));
        message.Subject = "Notificação da Aplicação";
        message.Body = new TextPart("html") { Text = body };

        using (var client = new SmtpClient())
        {
            try
            {
                Console.WriteLine("Connecting to MailerSend SMTP server...");
                await client.ConnectAsync("smtp.mailersend.net", 587, SecureSocketOptions.StartTls);

                Console.WriteLine("Authenticating...");
                await client.AuthenticateAsync("MS_muqbtG@trial-neqvygm8vw540p7w.mlsender.net", "Pz6VAdfI57GHFhk3");

                await client.SendAsync(message);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                Console.WriteLine("Disconnected from SMTP server.");
            }
        }
    }
}
