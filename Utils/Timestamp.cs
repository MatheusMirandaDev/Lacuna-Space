﻿namespace Luma.Utils;

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

    // Método para codificar o timestamp com base no tipo de codificação informado
    public static string EncodeTimestamp(long ticks, string encoding)
    {
        // seleciona o tipo de codificação com base no encoding informado
        return encoding switch
        {
            // Codifica os ticks no formato ISO8601
            "Iso8601" => new DateTimeOffset(ticks, TimeSpan.Zero).ToString("o"),

            // Codifica os ticks como string numérica
            "Ticks" => ticks.ToString(),

            // Codifica os ticks no formato binário little-endian em Base64
            "TicksBinary" => Convert.ToBase64String(BitConverter.GetBytes(ticks)),

            // Codifica os ticks no formato binário big-endian em Base64
            "TicksBinaryBigEndian" => Convert.ToBase64String(BitConverter.GetBytes(ticks).Reverse().ToArray()),

            // Lança exceção se o encoding não for suportado
            _ => throw new NotSupportedException($"Encoding não suportado: {encoding}"),
        };
    }
}
