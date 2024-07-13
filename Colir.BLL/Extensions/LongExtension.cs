namespace Colir.BLL.Extensions;

public static class LongExtension
{
    public static string ToHexColor(this long value)
    {
        return "#" + value.ToString("x6");
    }
}