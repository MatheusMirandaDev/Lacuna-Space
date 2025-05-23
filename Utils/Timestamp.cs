namespace Luma.Utils;

// Classe para lidar com o Timestamp
public class Timestamp
{
    // Método para decodificar o timestamp com base no tipo de codificação informado
    public static long DecodeTimestamp(string value, string encoding)
    {
        // seleciona o tipo de codificação com base no encoding informado
        return encoding switch
        {
            // Decodifica no formato ISO8601 para ticks
            "Iso8601" => DateTimeOffset.Parse(value).Ticks,

            // Decodificao no formato ticks
            "Ticks" => long.Parse(value),

            // Decodifica little-endian para ticks
            "TicksBinary" => BitConverter.ToInt64(Convert.FromBase64String(value), 0),

            // Decodifica big-endian para ticks
            "TicksBinaryBigEndian" => BitConverter.ToInt64(Convert.FromBase64String(value).Reverse().ToArray(), 0),

            // Lança exceção se o encoding não for suportado
            _ => throw new NotSupportedException($"Encoding Invalido: {encoding}")
        };
    }

    public static string EncodeTimestamp(long ticks, string encoding)
    {
        return encoding switch
        {
            "Iso8601" => new DateTimeOffset(ticks, TimeSpan.Zero).ToString("o"),
            "Ticks" => ticks.ToString(),
            "TicksBinary" => Convert.ToBase64String(BitConverter.GetBytes(ticks)),
            "TicksBinaryBigEndian" => Convert.ToBase64String(BitConverter.GetBytes(ticks).Reverse().ToArray()),
            _ => throw new NotSupportedException($"Encoding não suportado: {encoding}"),
        };
    }
}
