namespace CalendarApp.Background.Persistence.Entities;

public class WorkDayCheck
{
    public required string CountryCode { get; set; }
    public required DateOnly Date { get; set; }
    public required bool IsWorkDay { get; set; }
}
