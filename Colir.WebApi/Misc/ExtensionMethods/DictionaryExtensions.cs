using DAL.Enums;

namespace Colir.Misc.ExtensionMethods;

public static class DictionaryExtensions
{
    public static void RemoveByValue(this Dictionary<string, (string, UserAuthType)> src, string value)
    {
        foreach (var item in src.Where(kvp => kvp.Value.Equals(value)).ToList())
        {
            src.Remove(item.Key);
        }
    }
}