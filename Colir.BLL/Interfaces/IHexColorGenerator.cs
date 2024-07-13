namespace Colir.BLL.Interfaces;

public interface IHexColorGenerator
{
    long GetUniqueHexColor();
    List<long> GetUniqueHexColorsList(int count);
}