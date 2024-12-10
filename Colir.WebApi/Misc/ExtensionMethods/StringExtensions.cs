namespace Colir.Misc.ExtensionMethods;

public static class StringExtensions
{
    public static bool ContainsOneOf(this string source, params string[] values)
    {
        if (!source.Any() || !values.Any())
        {
            return false;
        }

        if (values.Any(value => source.Contains(value)))
        {
            return true;
        }

        return false;
    }
}