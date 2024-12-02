namespace Service.Notification.Domain
{
    public class MessageNotification
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Message { get; set; }
        public string? Email { get; set; }
    }
}
