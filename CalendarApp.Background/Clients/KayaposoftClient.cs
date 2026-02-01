using System.Diagnostics;
using CalendarApp.Background.Dtos.Kayaposoft;

namespace CalendarApp.Background.Clients;

public class KayaposoftClient(
    HttpClient httpClient,
    ILogger<KayaposoftClient> logger)
{
    public async Task<bool> IsWorkDayAsync(string countryCode, DateOnly date)
    {
        /*
         Operation isWorkDay
         Returns if given day is work day in given country.
         Example: URL to check if 30 July 2022 is work day in Hungary: 
             https://kayaposoft.com/enrico/json/v2.0?action=isWorkDay&date=30-07-2022&country=hun
         */
        
        Activity? span = Activity.Current?.Source.StartActivity("KayaposoftClient.IsWorkDayAsync");
        span?.SetTag("countryCode", countryCode);
        
        string formattedDate = date.ToString("dd-MM-yyyy");

        span?.SetTag("date", formattedDate);
        
        logger.LogInformation("Calling Kayaposoft IsWorkDay for {countryCode} at {date}", countryCode, date);
        
        IsWorkDayResponse? response = await httpClient.GetFromJsonAsync<IsWorkDayResponse>(
            $"?action=isWorkDay&date={formattedDate}&country={countryCode}");
        
        logger.LogInformation("Received IsWorkDayResponse from {countryCode} at {date} - {isWorkDay}",
            countryCode,
            date,
            response?.IsWorkDay);
        
        return response?.IsWorkDay ?? throw new Exception("Kayaposoft request failed");
    }
}
