using Colir.ApiRelatedServices.Models;

namespace Colir.Misc.ExtensionMethods;

public static class DictionaryExtensions
{
    public static void RemoveByValue(this Dictionary<string, RegistrationUserData> src, RegistrationUserData value)
    {
        foreach (var item in src.Where(kvp => kvp.Value.Equals(value)).ToList())
        {
            src.Remove(item.Key);
        }
    }
}