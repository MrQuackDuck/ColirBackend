namespace Colir.BLL.Tests.Interfaces;

public interface IHexColorGeneratorTests
{
    Task GetUniqueHexColorAsync_ReturnsHexInValidFormat();

    Task GetUniqueHexColorAsyncsListAsync_ReturnsCorrectAmountOfHexs();
    Task GetUniqueHexColorAsyncsListAsync_ReturnsHexsInValidFormat();
    Task GetUniqueHexColorAsyncsListAsync_ThrowsArgumentOutOfRangeException_WhenCountIsBelowZero();
}