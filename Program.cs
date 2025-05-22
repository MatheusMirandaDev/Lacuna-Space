using Luma.Models;
using Luma.Services;

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
                var probeNow = await client.GetProbeNowAsync(accessToken, probe.Id, probe.Encoding);

                // Verifica se a resposta da sincronização não é nula
                if (probeNow is not null) 
                {
                    // Converte o horário sincronizado (ticks) para DateTimeOffset (UTC)
                    var probeNowDateTime = new DateTimeOffset(probeNow.Value, TimeSpan.Zero);
                }
                else
                {
                    // Se a resposta for nula, exibe uma mensagem de erro
                    Console.WriteLine($"Falha ao sincronizar a sonda {probe.Name}.");
                }
            }

            Console.WriteLine("-------- 6. Jobs --------\n");
            var job = await client.TakeJobAsync(accessToken);

            if (job is not null)
            {
                Console.WriteLine($"Job ID: {job.Id}");
                Console.WriteLine($"Nome da probe: {job.ProbeName}");
            }
            else {
                Console.WriteLine("Nenhum job encontrado!");
            }

        }
        catch (Exception ex)
        {
            // Se ocorrer um erro, exibe a mensagem de erro
            Console.WriteLine($"Erro: {ex.Message}");
        }

    }
}
