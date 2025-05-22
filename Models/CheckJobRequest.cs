namespace Luma.Models;

public class CheckJobRequest
{
    public string ProbeNow { get; set; } = string.Empty;

    public long RoundTrip { get; set; } 
}
