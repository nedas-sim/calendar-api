using System.Text.Json.Serialization;

namespace CalendarApp.Background.Dtos.Kayaposoft;

public class IsWorkDayResponse
{
    [JsonPropertyName("isWorkDay")]
    public bool IsWorkDay { get; init; }
}
