using Luma.Services;

namespace Luma;

class Program
{
    public static async Task Main(string[] args)
    {
        const string username = "Matheus_Miranda";
        const string email = "email@email.com";


        // Cria o HttpClient
        var httpClient = new HttpClient();

        // Instancia o cliente da Luma
        var client = new LumaClient(httpClient);

        // Inicia o processo de autenticação
        try
        {
            // Faz a requisição para a API e recebe o token de acesso
            Console.WriteLine("------ 2. Start API ------\n");
            var accessToken = await client.StartAsync(username, email);

            // Acessa a lista de probes com o token de acesso e exibe os dados
            Console.WriteLine("\n------ 3. List Probes ------\n");
            var listProbes = await client.GetProbesAsync(accessToken);

            if (listProbes is not null && listProbes.Any())
            {

                foreach (var probe in listProbes)
                {
                    Console.WriteLine($"ID: {probe.Id}");
                    Console.WriteLine($"Name: {probe.Name}");
                    Console.WriteLine($"Encoding: {probe.Encoding}");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine();
                }
                Console.WriteLine("Fim da lista");
            }
            else
            {
                Console.WriteLine("Erro ao encontrar as Probes! ");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }

    }
}
