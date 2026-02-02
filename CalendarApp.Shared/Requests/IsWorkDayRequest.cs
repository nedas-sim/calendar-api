namespace CalendarApp.Shared.Requests;

public record IsWorkDayRequest
{
    public required string CountryCode { get; init; }
    public required DateOnly Date { get; init; }
}
