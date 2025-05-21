using Luma.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Luma.Services;

public class LumaClient
{
    /// HttpClient para acessar a API
    private readonly HttpClient _httpClient;

    public LumaClient(HttpClient httpClient)
    {
        // injeção do HttpClient
        _httpClient = httpClient;

        // configuração do HttpClient para acessar a API
        httpClient.BaseAddress = new Uri("https://luma.lacuna.cc/");

        // adiciona o cabeçalho da requisições para aceitar JSON
        _httpClient.DefaultRequestHeaders.Accept.Add(
           new MediaTypeWithQualityHeaderValue("application/json"));
    }

    // método para iniciar o processo de autenticação
    public async Task<string?> StartAsync(string username, string email)
    {
        // cria o objeto de requisição
        var request = new StartRequest
        {
            Username = username,
            Email = email
        };

        // faz a requisição para a API
        var response = await _httpClient.PostAsJsonAsync("api/start", request);

        // faz a leitura da resposta
        var result = await response.Content.ReadFromJsonAsync<StartResponse>();

        // verifica se a resposta é válida
        if (result is not null && result.Code == "Success")
        {
            // retorna o token de acesso
            return result.AccessToken;
        }

        // se a resposta não é válida, exibe o erro
        Console.WriteLine($"Erro: {result?.Code} - {result?.Message}");
        return null;

    }
}
