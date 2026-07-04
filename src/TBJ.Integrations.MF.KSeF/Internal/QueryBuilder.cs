using System.Text;

namespace TBJ.Integrations.MF.KSeF.Internal;

/// <summary>
/// Pomocnik do budowania query string dla zapytań GET do KSeF API.
/// </summary>
internal static class QueryBuilder
{
    /// <summary>
    /// Buduje query string z kolekcji parametrów, pomijając puste wartości.
    /// </summary>
    /// <param name="parameters">Słownik parametrów zapytania.</param>
    /// <returns>Query string z prefiksem <c>?</c> lub pusty ciąg, gdy brak parametrów.</returns>
    public static string Build(IReadOnlyDictionary<string, string?> parameters)
    {
        if (parameters.Count == 0)
            return string.Empty;

        var sb = new StringBuilder("?");
        var first = true;

        foreach (var (key, value) in parameters)
        {
            if (string.IsNullOrWhiteSpace(value))
                continue;

            if (!first)
                sb.Append('&');

            sb.Append(Uri.EscapeDataString(key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(value));
            first = false;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Buduje query string z listy parametrów przekazanych jako krotki.
    /// </summary>
    public static string Build(params (string Key, string? Value)[] parameters)
    {
        return Build(parameters.ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase));
    }
}
