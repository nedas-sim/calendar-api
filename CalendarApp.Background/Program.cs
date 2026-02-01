using CalendarApp.Background.Clients;
using CalendarApp.Background.Features.IsWorkDay;
using CalendarApp.Background.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<KayaposoftClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Kayaposoft:ApiUrl"]!);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddIsWorkDayServices();

WebApplication app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapIsWorkDay();

app.Run();
