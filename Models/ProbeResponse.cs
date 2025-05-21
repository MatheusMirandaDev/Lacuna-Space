namespace Luma.Models;

public class ProbeResponse
{
    public List<Probe>? Probes { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Message { get; set; }
}
