namespace Luma.Models;

// Modelo que representa um Job
public class Job
{
    // Identificador único do Job
    public string Id { get; set; } = string.Empty;


    // Nome da probe referente ao Job associado
    public string ProbeName { get; set; } = string.Empty;
}
