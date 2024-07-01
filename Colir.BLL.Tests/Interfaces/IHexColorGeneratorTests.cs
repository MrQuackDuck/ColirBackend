namespace Colir.BLL.Tests.Interfaces;

public interface IHexColorGeneratorTests
{
    Task GetUniqueHexColor_ReturnsUniqueHex();
    Task GetUniqueHexColor_ReturnsHexInValidFormat();

    Task GetUniqueHexColorsList_ReturnsCorrectAmountOfUniqueHexs();
    Task GetUniqueHexColorsList_ReturnsHexsInValidFormat();
    Task GetUniqueHexColorsList_ThrowsArgumentOutOfRangeException_WhenCountIsBelowZero();
}