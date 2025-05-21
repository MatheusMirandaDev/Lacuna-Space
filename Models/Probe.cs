namespace Luma.Models;

public class Probe
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Encoding { get; set; } = string.Empty;

    public double? TimeDilationFactor { get; set; } // Para o nivel extra
}
