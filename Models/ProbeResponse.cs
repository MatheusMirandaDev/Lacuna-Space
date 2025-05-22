namespace Luma.Models;

// Modelo que representa a resposta da API ao solicitar a lista de probes
public class ProbeResponse
{
    // Lista de probes retornadas pela API
    public List<Probe>? Probes { get; set; }

    // Código de resposta da API ['Success', 'Error', 'Unauthorized']
    public string Code { get; set; } = string.Empty;

    // Mensagem adicional da resposta, se houver
    public string? Message { get; set; }
}