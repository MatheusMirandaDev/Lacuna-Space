using Luma.Models;
using Luma.Utils;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Luma.Services;

public class LumaClient
{
    /// HttpClient para acessar a API
    private readonly HttpClient _httpClient;

    // construtor da classe com injeção de dependência do HttpClient
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

        // Faz uma requisição POST para o endpoint "api/start"
        var response = await _httpClient.PostAsJsonAsync("api/start", request);

        // Lê e desserializa o conteúdo da resposta JSON para um objeto do tipo StartResponse

        var result = await response.Content.ReadFromJsonAsync<StartResponse>();

        // verifica se a resposta é válida
        if (result is null)
        {
            // se a resposta não é válida, exibe o erro
            Console.WriteLine($"Erro: {result?.Code} - {result?.Message}");
            return null;
        }

        // Mostra o resultado da autenticação
        Console.WriteLine($"Code: {result.Code}");
        Console.WriteLine();
        Console.WriteLine($"Token: {result.AccessToken}");

        // retorna o token de acesso
        return result.AccessToken;
    }

    // [3.LIST PROBES] - método para listar as probes
    public async Task<List<Probe>?> GetProbesAsync(string acessToken)
    {

        // passa o token de acesso no cabeçalho da requisição HTTP (autenticação Bearer)
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", acessToken);

        // Faz uma requisição GET para o endpoint "api/probe"
        var response = await _httpClient.GetAsync("api/probe");

        // verifica se a resposta é válida
        if (response.IsSuccessStatusCode)
        {

            // Lê e desserializa o conteúdo da resposta JSON para um objeto do tipo ProbeResponse
            var result = await response.Content.ReadFromJsonAsync<ProbeResponse>();

            //Mostra o code obtido retornado pela API
            Console.WriteLine($"Code: {result?.Code}");

            if (result.Code == "Success")
            {
                // Se o código for sucesso, retorna as probes
                Console.WriteLine($"\nProbes encontradas: {result.Probes?.Count}");
                return result.Probes;
            }
            else
            {
                // Se o código não for sucesso, exibe a mensagem de erro
                Console.WriteLine("Erro ao buscar probes.");
                return null;
            }
        }
        else
        {
            //Se a requisição falhar, exibe o erro
            Console.WriteLine($"Erro HTTP: {response.StatusCode}");
            return null;
        }
    }

    // [4. TIMESTAMP] - método para decodificar o timestamp
    public async Task<(long Offset, long RoundTrip)?> SyncAsync(string accessToken, string probeId, string encoding)
    {
        // passa o token de acesso no cabeçalho da requisição HTTP (autenticação Bearer)
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        // t0 é o timestamp do cliente (UTC) antes do envio sa solicitação de sincronização
        var t0 = DateTimeOffset.UtcNow.Ticks;

        // faz uma requisição POST para o endpoint "api/probe/{probeId}/sync"
        var response = await _httpClient.PostAsync($"api/probe/{probeId}/sync", null);

        // t3 é o timestamp do cliente (UTC) imediatamente após a resposta da API
        var t3 = DateTimeOffset.UtcNow.Ticks;

        // Lê e desserializa o conteúdo da resposta JSON para um objeto do tipo SyncResponse
        var result = await response.Content.ReadFromJsonAsync<SyncResponse>();

        // verifica se o resultado está nulo
        if (result is null)
        {
            // se o resultado for nulo, exibe o erro
            Console.WriteLine($"Erro ao Sincronizar: {result?.Code} - {result?.Message}");
            return null;
        }

        // t1 e t2 são os timestamps retornados pela API após receber a requisição e antes de enviar a resposta
        var t1 = Timestamp.DecodeTimestamp(result.T1, encoding);
        var t2 = Timestamp.DecodeTimestamp(result.T2, encoding);

        // calcula o offset e o round-trip time
        var offset = ((t1 - t0) + (t2 - t3)) / 2;
        var roundTrip = (t3 - t0) - (t2 - t1);

        return (offset, roundTrip);
    }

    // [5.SYNC CLOCK] - método para sincronizar os relógios 
    public async Task<long?> GetProbeNowAsync(string accessToken, String probeId, string encoding)
    {
        // número máximo de tentativas para sincronização com round-trip menor que 5ms
        const int maxAttempts = 10;

        // Define o valor máximo de round-trip permitido em ticks (5 ms)
        const long maxRoundTripTicks = 5 * TimeSpan.TicksPerMillisecond;

        // variável para armazenar o melhor offset e round-trip enconrado
        long? bestOffset = null;
        long bestRoundTrip = long.MaxValue;

        // Loop de tentativas para sincronizar o relógio, buscando o menor round-trip possível
        for (int i = 0; i < maxAttempts; i++)
        {
            // Chama o método de sincronização e obtém o offset e round-trip da tentativa
            var result = await SyncAsync(accessToken, probeId, encoding);

            // se falhou, tenta novamente
            if (result is null) continue;

            // Se o resultado não for nulo, extrai o offset e round-trip
            var (offset, roundTrip) = result.Value;

            // se encontou um round-trip menor que o melhor encontrado até agora, atualiza o melhor offset e round-trip
            if (roundTrip < bestRoundTrip)
            {
                bestRoundTrip = roundTrip;
                bestOffset = offset;
            }
        }
        // Exibe mensagem informando que o round-trip  atingiu o valor ideal
        if (bestRoundTrip <= maxRoundTripTicks)
        {
            Console.WriteLine($"Round-trip perfeito: {TimeSpan.FromTicks(bestRoundTrip).TotalMilliseconds} ms");
        }
        else
        {
            // Se o round-trip não for perfeito, exibe a mensagem informando o melhor round-trip encontrado
            Console.WriteLine($"** Melhor round-trip foi: {TimeSpan.FromTicks(bestRoundTrip).TotalMilliseconds} ms (acima do ideal) **");
        }

        //  Se o melhor offset for nulo, significa que não foi possível sincronizar
        if (bestOffset is null)
        {
            Console.WriteLine("Não foi possivel fazer a sincronização");
            return null;
        }

        // Calcula o horário sincronizado (ticks atual + melhor offset)
        var probeNow = DateTimeOffset.UtcNow.Ticks + bestOffset.Value;
        Console.WriteLine($"Horário atual da probe: {new DateTimeOffset(probeNow, TimeSpan.Zero)} UTC");

        return probeNow;
    }

    // [6. JOBS] - método para pegar um job
    public async Task<Job?> TakeJobAsync(string accessToken)
    {
        // passa o token de acesso no cabeçalho da requisição HTTP (autenticação Bearer)
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        // faz uma requisição POST para o endpoint "api/job/take"
        var response = await _httpClient.PostAsync("api/job/take", null);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Erro ao pegar Job: {response.StatusCode}");
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<TakeJobResponse>();

        if (result is null)
        {
            Console.WriteLine($"Falha ao pegar o Job: {result?.Code} - {result?.Message}");
            return null;
        }

        return result?.Job;
    }

    // [7. CHECK JOB] - método para enviar o resultado do job
    public async Task<CheckJobResponse?> CheckJobAsync(string accessToken, string jobId, CheckJobRequest request)
    {
        // passa o token de acesso no cabeçalho da requisição HTTP (autenticação Bearer)
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        // faz uma requisição POST para o endpoint "api/job/{id}/check" enviando o corpo da requisição
        var response = await _httpClient.PostAsJsonAsync($"api/job/{jobId}/check", request);

        // Lê e desserializa o conteúdo da resposta JSON para um objeto do tipo CheckJobResponse
        var result = await response.Content.ReadFromJsonAsync<CheckJobResponse>();

        if (result is null)
        {
            Console.WriteLine("Erro ao enviar o resultado do job!");
            return null;
        }

        return result;
    }
}
