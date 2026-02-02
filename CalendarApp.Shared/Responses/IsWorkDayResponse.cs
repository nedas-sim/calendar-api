namespace CalendarApp.Shared.Responses;

public record IsWorkDayResponse
{
    public required bool IsWorkDay { get; init; }
}
