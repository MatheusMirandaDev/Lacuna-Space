

namespace Luma.Models;

public class StartResponse
{
    public string? AccessToken { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Message { get; set; }

}
