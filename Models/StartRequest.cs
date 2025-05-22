namespace Luma.Models;

// Modelo que representa a requisição para API ao iniciar a autenticação
public class StartRequest
{
    // Nome de usuário
    public string Username { get; set; } = string.Empty;

    // Email do usuário
    public string Email { get; set; } = string.Empty;
}