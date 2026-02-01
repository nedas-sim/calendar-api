using System.Diagnostics;
using CalendarApp.Background.Clients;
using CalendarApp.Background.Persistence;
using CalendarApp.Background.Persistence.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Background.Features.IsWorkDay;

public static class IsWorkDayEndpoint
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddIsWorkDayServices()
        {
            services.AddScoped<IsWorkDayHandler>();
            return services;
        }
    }
    
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapIsWorkDay()
        {
            endpoints.MapGet(
                "api/is-work-day", 
                async (
                    [FromQuery] string countryCode,
                    [FromQuery] DateOnly date,
                    [FromServices] IsWorkDayHandler handler) => Results.Ok(new
                    {
                        IsWorkDay = await handler.IsWorkDayAsync(countryCode, date),
                    }));
            
            return endpoints;
        }
    }
}

public class IsWorkDayHandler(
    AppDbContext dbContext,
    KayaposoftClient kayaposoftClient,
    ILogger<IsWorkDayHandler> logger)
{
    public async Task<bool> IsWorkDayAsync(
        string countryCode,
        DateOnly date)
    {
        using Activity? span = Activity.Current?.Source.StartActivity("is-work-day endpoint");
        
        logger.LogInformation("Received is-work-day request for {countryCode} at {date}", countryCode, date);
        
        bool? checkFromDb = await GetFromDatabaseAsync(countryCode, date);

        if (checkFromDb is not null)
        {
            logger.LogInformation("Found {isWorkDay} value in the database", checkFromDb);
            return checkFromDb.Value;
        }

        bool isWorkDay = await CheckExternallyAndSaveAsync(countryCode, date);

        logger.LogInformation("Fetched {isWorkDay} from external service and saved to the database", isWorkDay);
        
        return isWorkDay;
    }

    private async Task<bool?> GetFromDatabaseAsync(
        string countryCode,
        DateOnly date)
    {
        using Activity? _ = Activity.Current?.Source.StartActivity("is-work-day lookup to the database");
        
        WorkDayCheck? check = await dbContext
            .WorkDayChecks
            .AsNoTracking()
            .Where(x => x.CountryCode == countryCode && x.Date == date)
            .FirstOrDefaultAsync();

        return check?.IsWorkDay;
    }

    private async Task<bool> CheckExternallyAndSaveAsync(
        string countryCode,
        DateOnly date)
    {
        bool isWorkDay = await kayaposoftClient.IsWorkDayAsync(countryCode, date);

        using Activity? _ = Activity.Current?.Source.StartActivity("is-work-day value persistence to the database");
        
        dbContext.WorkDayChecks.Add(new WorkDayCheck
        {
            CountryCode = countryCode,
            Date = date,
            IsWorkDay = isWorkDay,
        });
                    
        await dbContext.SaveChangesAsync();

        return isWorkDay;
    }
}
