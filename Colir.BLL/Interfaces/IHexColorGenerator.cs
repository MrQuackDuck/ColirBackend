namespace Colir.BLL.Interfaces;

public interface IHexColorGenerator
{
    /// <summary>
    /// Gets a unique Hex Id
    /// </summary>
    Task<int> GetUniqueHexColorAsync();

    /// <summary>
    /// Gets a range of unique Hex Ids
    /// </summary>
    /// <param name="count">Count of hexs to get</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the count is below zero</exception>
    Task<List<int>> GetUniqueHexColorAsyncsListAsync(int count);
}