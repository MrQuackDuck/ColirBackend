namespace Colir.BLL.Interfaces;

public interface IHexColorGenerator
{
    Task<int> GetUniqueHexColor();
    Task<List<int>> GetUniqueHexColorsList(int count);
}