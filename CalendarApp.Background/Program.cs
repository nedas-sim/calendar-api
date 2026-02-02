using CalendarApp.Background.Clients;
using CalendarApp.Background.Features;
using CalendarApp.Background.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<KayaposoftClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Kayaposoft:ApiUrl"]!);
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

WebApplication app = builder.Build();

app.Run();
