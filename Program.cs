using Luma.Models;
using Luma.Services;
using Luma.Utils;

namespace Luma;

class Program
{
    public static async Task Main()
    {
        const string username = "Matheus_Miranda"; // Nome de usuário
        const string email = "email@email.com"; // Email do usuário


        // Cria o HttpClient para fazer as requisições HTTP
        var httpClient = new HttpClient();

        // Instancia o cliente da Luma para interagir com a API
        var client = new LumaClient(httpClient);

        try
        {
            // Inicia a autenticação e obtém o token de acesso
            Console.WriteLine("-------- 2. Start API --------\n");
            var accessToken = await client.StartAsync(username, email); // armazena o token de acesso

            // Solicita a lista de sondas (probes) disponíveis usando o token de acesso
            Console.WriteLine("\n-------- 3. List Probes --------\n");

            // salva a lista de probes retornada pela API
            var listProbes = await client.GetProbesAsync(accessToken);

            // Verifica se a lista de probes não é nula
            if (listProbes is not null)
            {
                // Exibe os dados de cada probe encontrada
                foreach (var probe in listProbes)
                {
                    Console.WriteLine($"ID: {probe.Id}");
                    Console.WriteLine($"Name: {probe.Name}");
                    Console.WriteLine($"Encoding: {probe.Encoding}");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine();
                }
                Console.WriteLine("****** Fim da Lista ******");
            }
            else
            {
                // Se a lista de probes for nula, exibe uma mensagem de erro
                Console.WriteLine("Erro ao encontrar as Probes! ");
            }

            Console.WriteLine($"\n\n-------- 5. Clock Synchronization --------");
            // Para cada sonda, executa o processo de sincronização do relógio
            foreach (var probe in listProbes)
            {
               
                Console.WriteLine($"\n------ Sincronizando a sonda {probe.Name} ------\n");

                // armazena o horário sincronizado atual da sonda
                var probeNowTicks = await client.GetProbeNowAsync(accessToken, probe.Id, probe.Encoding);

                // Verifica se a resposta da sincronização não é nula
                if (probeNowTicks is not null) 
                {
                    // Converte o horário sincronizado (ticks) para DateTimeOffset (UTC)
                    var probeNowDateTime = new DateTimeOffset(probeNowTicks.Value, TimeSpan.Zero);
                }
                else
                {
                    // Se a resposta for nula, exibe uma mensagem de erro
                    Console.WriteLine($"Falha ao sincronizar a sonda {probe.Name}.");
                }
            }

            Console.WriteLine("\n-------- 6. Jobs --------\n");
            bool done = false;
            while (!done)
            { 
                var job = await client.TakeJobAsync(accessToken);

                if (job is not null)
                {

                    // Encontra a probe do job pelo nome
                    var jobProbe = listProbes?.FirstOrDefault(p => p.Name == job.ProbeName);
                    if (jobProbe == null)
                    {
                        Console.WriteLine("Probe do job não encontrada!");
                        break;
                    }
                    Console.WriteLine($"\n------ Execução o job para a Probe: {jobProbe.Name} ------\n");
                    Console.WriteLine($"Job ID: {job.Id}");

                    var syncResult = await client.SyncAsync(accessToken, jobProbe.Id, jobProbe.Encoding);

                    if (syncResult is null)
                    {
                        Console.WriteLine("Falha ao sincronizar a probe do job!");
                        break;
                    }

                    var (offset, roundTrip) = syncResult.Value;
                    var probeNowTicks = DateTimeOffset.UtcNow.Ticks + offset;
                    var probeNowStr = Timestamp.EncodeTimestamp(probeNowTicks, jobProbe.Encoding);

                    if (string.IsNullOrEmpty(probeNowStr))
                    {
                        Console.WriteLine("Erro ao codificar o timestamp da probe do job!");
                        return;
                    }

                    var checkRequest = new CheckJobRequest
                    {
                        ProbeNow = probeNowStr,
                        RoundTrip = roundTrip
                    };

                    var checkResponse = await client.CheckJobAsync(accessToken, job.Id, checkRequest);
                    Console.WriteLine($"CheckJob code: {checkResponse?.Code} - {checkResponse?.Message}");

                    if (checkResponse?.Code == "Done")
                    {
                        Console.WriteLine("\n-----------------------------------\n");
                        Console.WriteLine("Código de resposta == 'Done'");
                        Console.WriteLine("Você passou!");
                        done = true;
                    }
                }
                else
                {
                    Console.WriteLine("Nenhum job encontrado!");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            // Se ocorrer um erro, exibe a mensagem de erro
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
