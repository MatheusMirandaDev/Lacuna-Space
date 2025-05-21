using Luma.Services;

namespace Luma;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("------ Start API ------");

        const string username = "Matheus_Miranda";
        const string email = "email@email.com";


        // Cria o HttpClient
        var httpClient = new HttpClient();

        // Instancia o cliente da Luma
        var client = new LumaClient(httpClient);

        // Faz o start
        

        try
        {
            var accessToken = await client.StartAsync(username, email);
            Console.WriteLine($"Access Token: {accessToken}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }

    }
}
