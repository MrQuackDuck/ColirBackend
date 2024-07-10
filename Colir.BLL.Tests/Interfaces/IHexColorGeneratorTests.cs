namespace Colir.BLL.Tests.Interfaces;

public interface IHexColorGeneratorTests
{
    Task GetUniqueHexColor_ReturnsHexInValidFormat();
    
    Task GetUniqueHexColorsList_ReturnsCorrectAmountOfHexs();
    Task GetUniqueHexColorsList_ReturnsHexsInValidFormat();
    Task GetUniqueHexColorsList_ThrowsArgumentOutOfRangeException_WhenCountIsBelowZero();
}