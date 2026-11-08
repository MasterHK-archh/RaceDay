namespace RaceDayMVC.Services;

public static class ApiErrorHelper
{
    /// The API returns error bodies as either plain text or a JSON-quoted string
    /// (e.g. "Invalid email or password."). Strip quotes and trim so they render nicely.
    public static string Prettify(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "Something went wrong. Please try again.";
        var s = raw.Trim();
        if (s.Length >= 2 && s.StartsWith('"') && s.EndsWith('"'))
            s = s[1..^1];
        return s;
    }
}