
namespace Luma.Utils;

public class Timestamp
{
    public static long DecodeTimestamp(string value, string encoding)
    {
        return encoding switch
        {
            "Iso8601" => DateTimeOffset.Parse(value).Ticks,
            "Ticks" => long.Parse(value),
            "TicksBinary" => BitConverter.ToInt64(Convert.FromBase64String(value), 0),
            "TicksBinaryBigEndian" => BitConverter.ToInt64(Convert.FromBase64String(value).Reverse().ToArray(), 0),
            _ => throw new NotSupportedException($"Encoding Invalido: {encoding}")
        };
    }
}
