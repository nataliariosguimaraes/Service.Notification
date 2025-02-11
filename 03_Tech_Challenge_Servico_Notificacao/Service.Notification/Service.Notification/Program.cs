using Service.Notification;
using Service.Notification.Configuration;
using Service.Notification.Services;
using Service.Notification.Services.Contracts;

var builder = Host.CreateApplicationBuilder(args);

// Registrar o HttpClient
builder.Services.AddHttpClient();

// Registrar o EmailService com HttpClient
builder.Services.AddTransient<IEmailService, EmailService>();

// Registrar outros serviços
builder.Services.AddSingleton<IMessageBusService, MessageBusService>();
builder.Services.AddHostedService<Worker>();

// Configurar opções do MailerSend
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

builder.Services.Configure<MailerSendOptions>(configuration.GetSection("MailerSend"));

var host = builder.Build();
host.Run();