namespace Luma.Models;

public class TakeJobResponse
{
    // Lista de Jobs retornadas pela API
    public Job? Job { get; set; }

    // Código de resposta da API ['Success', 'Error', 'Unauthorized']
    public string Code { get; set; } = string.Empty;

    // Mensagem adicional da resposta, se houver
    public string? Message { get; set; }
}
