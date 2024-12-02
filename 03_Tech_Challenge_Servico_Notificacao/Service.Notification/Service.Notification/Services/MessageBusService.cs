using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Service.Notification.Domain;
using Service.Notification.Services.Contracts;
using System.Text;
using System.Text.Json;

namespace Service.Notification.Services;

public class MessageBusService : IMessageBusService
{
    private const string QUEUE_NOTIFICATION = "fila-notificacoes";
    private readonly IEmailService _emailService;
    private readonly ConnectionFactory _factory;

    public MessageBusService(IEmailService emailService)
    {
        _emailService = emailService;
        _factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };
    }

    public async Task ProcessQueue(CancellationToken cancellationToken)
        {
        Console.WriteLine("Starting to process queue...");

        using (var connection = _factory.CreateConnection())
        {
            Console.WriteLine("RabbitMQ connection established.");

            using (var channel = connection.CreateModel())
            {
                Console.WriteLine("Channel created.");

                channel.QueueDeclare(
                    queue: QUEUE_NOTIFICATION,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                Console.WriteLine($"Queue '{QUEUE_NOTIFICATION}' declared.");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (sender, eventArgs) =>
                {
                    Console.WriteLine("Message received from queue.");

                    try
                    {
                        var messageBytes = eventArgs.Body.ToArray();
                        var messageJson = Encoding.UTF8.GetString(messageBytes);

                        Console.WriteLine($"Raw message: {messageJson}");

                        var messageNotification = JsonSerializer.Deserialize<MessageNotification>(messageJson);
                        if (messageNotification == null)
                        {
                            throw new Exception("Failed to deserialize message.");
                        }

                        Console.WriteLine($"Message deserialized: Name={messageNotification.Name}, Email={messageNotification.Email}, Message={messageNotification.Message}");

                        await _emailService.SendEmailAsync(
                            messageNotification.Name,
                            messageNotification.Email,
                            messageNotification.Message
                        );

                        Console.WriteLine("Email sent successfully.");

                        // Confirmar processamento da mensagem
                        channel.BasicAck(eventArgs.DeliveryTag, false);
                        Console.WriteLine("Message acknowledged.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                        // Rejeitar mensagem e permitir reprocessamento
                        channel.BasicNack(eventArgs.DeliveryTag, false, true);
                    }
                };

                channel.BasicConsume(QUEUE_NOTIFICATION, false, consumer);
                Console.WriteLine($"Consumer started on queue '{QUEUE_NOTIFICATION}'.");

                try
                {
                    await Task.Delay(Timeout.Infinite, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Cancellation requested. Stopping consumer...");
                }
                finally
                {
                    channel.Close();
                    Console.WriteLine("Channel closed.");
                }
            }
        }

        Console.WriteLine("RabbitMQ connection closed.");
    }
}
