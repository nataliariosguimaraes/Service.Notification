using Service.Notification;
using Service.Notification.Services;
using Service.Notification.Services.Contracts;

var builder = Host.CreateApplicationBuilder(args);


// Registrar os serviços
builder.Services.AddSingleton<IMessageBusService, MessageBusService>();
builder.Services.AddSingleton<IEmailService, EmailService>();

// Registrar o Worker
builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();


