using System;
using System.Text;

namespace Tenon.Extensions.System;

public static class StringExtension
{
    public static bool CompareIgnoreCase(this string data, string compareData)
    {
        return string.Equals(data, compareData, StringComparison.OrdinalIgnoreCase);
    }

    public static string Escape(this string data)
    {
        if (string.IsNullOrEmpty(data)) return data;
        var builder = new StringBuilder();
        foreach (var c in data)
            builder.Append(char.IsLetterOrDigit(c)
                           || c == '-' || c == '_' || c == '\\'
                           || c == '/' || c == '.'
                ? c.ToString()
                : Uri.HexEscape(c));

        return builder.ToString();
    }

    public static string UnEscape(this string data)
    {
        if (string.IsNullOrEmpty(data)) return data;
        var builder = new StringBuilder();
        var count = data.Length;
        var index = 0;

        while (index != count)
            builder.Append(Uri.IsHexEncoding(data, index) ? Uri.HexUnescape(data, ref index) : data[index++]);

        return builder.ToString();
    }

    public static string Unique()
    {
        return Guid.NewGuid().ToString().Replace("-", string.Empty);
    }

    public static bool IsEmpty(this string data)
    {
        return string.IsNullOrEmpty(data) || string.IsNullOrWhiteSpace(data) || data.Length == 0 || data == "" ||
               string.Empty == data;
    }
}