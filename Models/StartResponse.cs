namespace Luma.Models;

// Modelo que representa a resposta da API ao iniciar a autenticação
public class StartResponse
{
    // Token de acesso retornado pela API
    public string? AccessToken { get; set; }

    // Código de resposta da API ['Success', 'Error']
    public string Code { get; set; } = string.Empty;

    // Mensagem adicional da resposta, se houver
    public string? Message { get; set; }

}
