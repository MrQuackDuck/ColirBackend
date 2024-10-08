﻿using Colir.BLL.RequestModels.UserStatistics;
using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.BLL.Tests.Tests;

public class UserStatisticsServiceTests : IUserStatisticsServiceTests
{
    private ColirDbContext _dbContext;
    private UserStatisticsService _userStatisticsService;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize the service
        var configMock = new Mock<IConfiguration>();
        var roomFileMangerMock = new Mock<IRoomFileManager>();
        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object, roomFileMangerMock.Object);
        var mapper = AutomapperProfile.InitializeAutoMapper().CreateMapper();
        _userStatisticsService = new UserStatisticsService(unitOfWork, mapper);

        // Add entities
        UnitTestHelper.SeedData(_dbContext);
    }

    [TearDown]
    public void CleanUp()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetStatisticsAsync_ReturnsStats()
    {
        // Arrange
        var expectedStats = _dbContext.UserStatistics.First(s => s.UserId == 1).ToUserStatisticsModel();
        var request = new RequestToGetStatistics
        {
            IssuerId = 1
        };

        // Act
        var result = await _userStatisticsService.GetStatisticsAsync(request);

        // Assert
        Assert.That(result, Is.EqualTo(expectedStats).Using(new UserStatisticsModelEqualityComparer()));
    }

    [Test]
    public async Task GetStatisticsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var request = new RequestToGetStatistics
        {
            IssuerId = 404
        };

        // Act
        AsyncTestDelegate act = async () => await _userStatisticsService.GetStatisticsAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }
}