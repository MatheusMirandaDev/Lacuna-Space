namespace Luma.Models;

public class CheckJobResponse
{
    // Código de resposta da API ['Success', 'Done', 'Fail', 'Error', 'Unauthorized']
    public string Code { get; set; } = string.Empty;

    // Mensagem adicional da resposta, se houver
    public string? Message { get; set; }
}
