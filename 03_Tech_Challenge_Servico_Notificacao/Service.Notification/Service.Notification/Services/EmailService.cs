using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Service.Notification.Configuration;
using Service.Notification.Services.Contracts;

namespace Service.Notification.Services;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiToken;

    public EmailService(HttpClient httpClient, IOptions<MailerSendOptions> options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options), "MailerSend configuration is required.");
        }

        if (options.Value == null || string.IsNullOrWhiteSpace(options.Value.ApiToken))
        {
            throw new Exception("MailerSend API token is missing or empty.");
        }

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiToken = options.Value.ApiToken;
    }


    public async Task SendEmailAsync(string name, string email)
    {
        var mailerSendApiUrl = "https://api.mailersend.com/v1/email";
        using (var httpClient = new HttpClient())
        {
             httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiToken);
            var emailRequest = new
            {
                from = new
                {
                    email = "no-reply@techchallenge.com",
                    name = "Tech Challenge"
                },
                to = new[]
                {
                new
                {
                    email = email,
                    name = name
                }
            },
                subject = "Notificação da Aplicação",
                html = "Cadastro de contato"
            };

            var jsonContent = JsonSerializer.Serialize(emailRequest);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                Console.WriteLine("Sending email via MailerSend API...");
                var response = await _httpClient.PostAsync(mailerSendApiUrl, httpContent);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error sending email: {errorContent} token = {_apiToken}");
                    throw new Exception($"Failed to send email: {errorContent} token = {_apiToken}");
                }

                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
}