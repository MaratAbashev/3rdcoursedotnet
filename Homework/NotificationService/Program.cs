using Microsoft.AspNetCore.Server.Kestrel.Core;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5278, listenOpt =>
    {
        listenOpt.Protocols = HttpProtocols.Http2;
    });
});
// Add services to the container.
builder.Services.AddGrpc();
builder.Logging.ClearProviders()
    .AddConsole()
    .SetMinimumLevel(LogLevel.Information);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UserNotificationService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();