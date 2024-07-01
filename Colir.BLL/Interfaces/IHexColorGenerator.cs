namespace Colir.BLL.Interfaces;

public interface IHexColorGenerator
{
    string GetUniqueHexColor();
    List<string> GetUniqueHexColorsList(int count);
}