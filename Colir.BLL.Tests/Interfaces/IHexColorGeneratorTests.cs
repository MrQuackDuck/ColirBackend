namespace Colir.BLL.Tests.Interfaces;

public interface IHexColorGeneratorTests
{
    void GetUniqueHexColor_ReturnsUniqueHex();
    void GetUniqueHexColor_ReturnsHexInValidFormat();

    void GetUniqueHexColorsList_ReturnsCorrectAmountOfUniqueHexs();
    void GetUniqueHexColorsList_ReturnsHexsInValidFormat();
    void GetUniqueHexColorsList_ThrowsArgumentOutOfRangeException_WhenCountIsBelowZero();
}