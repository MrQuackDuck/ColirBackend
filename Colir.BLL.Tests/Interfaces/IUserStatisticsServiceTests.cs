namespace Colir.BLL.Tests.Interfaces;

public interface IUserStatisticsServiceTests
{
    void GetStatisticsAsync_ReturnsStats();
    void GetStatisticsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
}