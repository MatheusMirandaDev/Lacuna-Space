namespace Luma.Models;

// Modelo que representa uma sonda (probe)
public class Probe
{
    // Identificador único da probe
    public string Id { get; set; } = string.Empty;

    // Nome da probe
    public string Name { get; set; } = string.Empty;

    // Tipo de codificação utilizada pela probe
    public string Encoding { get; set; } = string.Empty;

    // Fator de dilatação do tempo da probe (usado para o nível extra)
    public double? TimeDilationFactor { get; set; } 
}