namespace Luma.Models;

// Classe que representa a requisição para checagem do clock de um job
public class CheckJobRequest
{
    // Timestamp atual sincronizado com a sonda, codificado no formato dela
    public string ProbeNow { get; set; } = string.Empty;

    // Round-trip delay calculado em ticks
    public long RoundTrip { get; set; }
}