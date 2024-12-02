namespace Service.Notification.Services.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(string name, string email, string body);
    }
}
