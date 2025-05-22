using Luma.Models;
using Luma.Utils;
using System.Diagnostics;
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

    // [2.START] - método para iniciar o processo de autenticação
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
        if (result is null )
        {
        // se a resposta não é válida, exibe o erro
            Console.WriteLine($"Erro: {result?.Code} - {result?.Message}");
            return null;
        }
        Console.WriteLine($"Code: {result.Code}");
        Console.WriteLine();
        Console.WriteLine($"Token: {result.AccessToken}");

        // retorna o token de acesso
        return result.AccessToken;
    }

    // [3.LIST PROBES] - método para listar as probes
    public async Task<List<Probe>?> GetProbesAsync(string acessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", acessToken);

        var response = await _httpClient.GetAsync("api/probe");
        if (response.IsSuccessStatusCode)
        {
            // faz a leitura do Json
            var result = await response.Content.ReadFromJsonAsync<ProbeResponse>();
            
            Console.WriteLine($"Code: {result?.Code}");
            if (result.Code == "Success")
            {
                Console.WriteLine($"\nProbes encontradas: {result.Probes?.Count}");
                return result.Probes;
            }

            Console.WriteLine("Erro ao buscar probes.");
            return null;
        }
        else
        {
            Console.WriteLine($"Erro HTTP: {response.StatusCode}");
            return null;
        }

    }

    // [4. TIMESTAMP] - método para decodificar o timestamp
    public async Task<(long Offset, long RoundTrip)?> SyncAsync(string accessToken, string probeId,  string encoding) 
    { 
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var t0 = DateTimeOffset.UtcNow.Ticks;

        var response = await _httpClient.PostAsync($"api/probe/{probeId}/sync", null);
        var t3 = DateTimeOffset.UtcNow.Ticks; //deve ser capturado imediatamente após a resposta da API

        var result = await response.Content.ReadFromJsonAsync<SyncResponse>();

        if (result is null)
        {
            Console.WriteLine($"Erro ao Sincronizar: {result?.Code} - {result?.Message}");
            return null;
        }

        var t1 = Timestamp.DecodeTimestamp(result.T1, encoding);
        var t2 = Timestamp.DecodeTimestamp(result.T2, encoding);

        var offset = ((t1 - t0) + (t2 - t3)) / 2;
        var roundTrip = (t3 - t0) - (t2 - t1) ;

        return (offset, roundTrip);
    }

    // [5.SYNC CLOCK] - método para sincronizar os relógios 
    public async Task<long?> GetProbeNowAsync(string accessToken, String probeId, string encoding)
    {
        const int maxAttempts = 10;
        const long maxRoundTripTicks = 5 * TimeSpan.TicksPerMillisecond;

        long? bestOffset = null;
        long bestRoundTrip = long.MaxValue;

        Console.WriteLine($"Sincronizando ...\n");

        for (int i = 0; i < maxAttempts; i++)
        {
            var result = await SyncAsync(accessToken, probeId, encoding);
            if (result is null) continue;

            var (offset, roundTrip) = result.Value;

            if(roundTrip < bestRoundTrip)
            {
                bestRoundTrip = roundTrip;
                bestOffset = offset;
            }
        }
        if (bestRoundTrip <= maxRoundTripTicks)
        {
            Console.WriteLine($"Round-trip perfeito: {TimeSpan.FromTicks(bestRoundTrip).TotalMilliseconds} ms");
        } else
        {
            Console.WriteLine($"** Melhor round-trip foi: {TimeSpan.FromTicks(bestRoundTrip).TotalMilliseconds} ms (acima do ideal) **");
        }

        if (bestOffset is null)
        {
            Console.WriteLine("Não foi possivel fazer a sincronização");
            return null;
        }

        var probeNow = DateTimeOffset.UtcNow.Ticks + bestOffset.Value;
        Console.WriteLine($"probeNow: {new DateTimeOffset(probeNow, TimeSpan.Zero)} UTC");

        return probeNow;
    }


}
