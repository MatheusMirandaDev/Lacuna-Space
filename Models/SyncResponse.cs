namespace Luma.Models;

// Modelo que representa a resposta da API de sincronização
public class SyncResponse
{
    // Timestamp da sonda imediatamente após receber a solicitação (codificado)
    public string T1 { get; set; } = string.Empty;

    // Timestamp da sonda imediatamente antes de enviar a resposta (codificado)
    public string T2 { get; set; } = string.Empty;

    // Código de resposta da API ['Success', 'Error', 'Unauthorized']
    public string Code { get; set; } = string.Empty;

    // Mensagem adicional da resposta, se houver
    public string? Message { get; set; }
}