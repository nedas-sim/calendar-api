using CalendarApp.Api.Features;
using CalendarApp.Shared.Requests;
using MassTransit;
using MassTransit.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Scalar.AspNetCore;
using ZiggyCreatures.Caching.Fusion;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddMassTransit(busConfig =>
{
    busConfig.SetKebabCaseEndpointNameFormatter();
    
    busConfig.AddRequestClient<IsWorkDayRequest>();

    busConfig.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "calendar-api:";
});

builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(new FusionCacheEntryOptions
    {
        Duration = TimeSpan.FromMinutes(1),
    })
    .WithSystemTextJsonSerializer()
    .WithDistributedCache(sp => sp.GetRequiredService<IDistributedCache>())
    .AsHybridCache();

builder.Services.AddIsWorkDayServices();

builder.Services.AddOpenTelemetry()
    .WithTracing(config =>
    {
        config.AddSource(DiagnosticHeaders.DefaultListenerName);
    });

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapIsWorkDayEndpoint();

app.Run();
