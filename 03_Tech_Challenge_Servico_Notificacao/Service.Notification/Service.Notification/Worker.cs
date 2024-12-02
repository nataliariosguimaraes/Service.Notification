using Service.Notification.Services.Contracts;

namespace Service.Notification
{
    public class Worker : BackgroundService
    {
        private readonly IMessageBusService _rabbitMqService;

        public Worker(IMessageBusService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Iniciar o serviço do RabbitMQ
                await _rabbitMqService.ProcessQueue(stoppingToken);

                // Tempo de espera entre as iterações
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
