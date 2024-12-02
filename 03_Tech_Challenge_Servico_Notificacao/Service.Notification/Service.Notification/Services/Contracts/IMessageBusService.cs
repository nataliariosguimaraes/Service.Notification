namespace Service.Notification.Services.Contracts
{
    public interface IMessageBusService
    {
        Task ProcessQueue(CancellationToken cancellationToken);
    }
}
