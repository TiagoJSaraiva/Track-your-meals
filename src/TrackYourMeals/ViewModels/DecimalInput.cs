using System.Globalization;

namespace TrackYourMeals.ViewModels;

internal static class DecimalInput
{
    public static string Format(decimal value)
    {
        return value.ToString("0.####", CultureInfo.CurrentCulture);
    }

    public static bool TryParseNonNegative(string text, out decimal value)
    {
        return TryParse(text, out value) && value >= 0;
    }

    public static bool TryParsePositive(string text, out decimal value)
    {
        return TryParse(text, out value) && value > 0;
    }

    private static bool TryParse(string text, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var trimmed = text.Trim();
        if (trimmed is "," or "." or "-" or "-," or "-.")
        {
            return false;
        }

        const NumberStyles styles = NumberStyles.Number;

        if (trimmed.Contains(','))
        {
            var normalizedComma = trimmed.Replace(',', '.');
            return decimal.TryParse(normalizedComma, styles, CultureInfo.InvariantCulture, out value);
        }

        if (decimal.TryParse(trimmed, styles, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        return decimal.TryParse(trimmed, styles, CultureInfo.CurrentCulture, out value);
    }
}
