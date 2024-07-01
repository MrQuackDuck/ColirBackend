namespace Colir.BLL.Tests.Interfaces;

public interface IUserStatisticsServiceTests
{
    Task GetStatisticsAsync_ReturnsStats();
    Task GetStatisticsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound();
}