using CalendarApp.Background.Clients;
using CalendarApp.Background.Features;
using CalendarApp.Background.Persistence;
using MassTransit;
using MassTransit.Logging;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<KayaposoftClient>(client =>
{
    client.BaseAddress = new Uri("https://Kayaposoft");
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddMassTransit(busConfig =>
{
    busConfig.SetKebabCaseEndpointNameFormatter();
    
    busConfig.AddConsumer<IsWorkDayConsumer>();
    
    busConfig.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddOpenTelemetry()
    .WithTracing(config =>
    {
        config.AddSource(DiagnosticHeaders.DefaultListenerName);
    });

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
