using CalendarApp.Shared.Requests;
using CalendarApp.Shared.Responses;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

namespace CalendarApp.Api.Features;

public static class IsWorkDay
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapIsWorkDayEndpoint()
        {
            endpoints.MapGet("api/is-work-day", async (
                [FromQuery] string countryCode,
                [FromQuery] DateOnly date,
                [FromServices] IsWorkDayHandler handler) =>
            {
                bool isWorkDay = await handler.IsWorkDayAsync(countryCode, date);

                return Results.Ok(new IsWorkDayResponse
                {
                    IsWorkDay = isWorkDay,
                });
            });

            return endpoints;
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddIsWorkDayServices()
        {
            services.AddScoped<IsWorkDayHandler>();
            return services;
        }
    }
}

file class IsWorkDayHandler(
    IRequestClient<IsWorkDayRequest> requestClient,
    HybridCache hybridCache)
{
    public async Task<bool> IsWorkDayAsync(string countryCode, DateOnly date)
    {
        string cacheKey = $"is-work-day/{countryCode}-{date}";

        return await hybridCache.GetOrCreateAsync(
            cacheKey, 
            async _ => await GetFromExternalSourceAsync(countryCode, date));
    }

    private async Task<bool> GetFromExternalSourceAsync(string countryCode, DateOnly date)
    {
        IsWorkDayRequest request = new()
        {
            CountryCode = countryCode,
            Date = date,
        };

        Response<IsWorkDayResponse> response = 
            await requestClient.GetResponse<IsWorkDayResponse>(request);

        return response.Message.IsWorkDay;
    }
}