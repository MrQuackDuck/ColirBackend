using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using DAL;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.BLL.Tests.Tests;

public class HexColorGeneratorTests : IHexColorGeneratorTests
{
    private ColirDbContext _dbContext;
    private HexColorGenerator _hexGenerator;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize the service
        var configMock = new Mock<IConfiguration>();
        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object);
        _hexGenerator = new HexColorGenerator(unitOfWork);

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
    public async Task GetUniqueHexColor_ReturnsHexInValidFormat()
    {   
        // Act
        var result = _hexGenerator.GetUniqueHexColor();
        
        // Assert
        Assert.That(result.ToString("x6").Length == 6);
    }

    [Test]
    public async Task GetUniqueHexColorsList_ReturnsHexsInValidFormat()
    {
        // Act
        var result = _hexGenerator.GetUniqueHexColorsList(5);
        
        // Assert
        foreach (var hex in result)
        {
            Assert.That(hex.ToString("x6").Length == 6);
        }
    }

    [Test]
    public async Task GetUniqueHexColorsList_ReturnsCorrectAmountOfHexs()
    {
        // Act
        var result = _hexGenerator.GetUniqueHexColorsList(5);

        // Assert
        Assert.That(result.Count() == 5);
    }

    [Test]
    public async Task GetUniqueHexColorsList_ThrowsArgumentOutOfRangeException_WhenCountIsBelowZero()
    {
        // Act
        TestDelegate act = () => _hexGenerator.GetUniqueHexColorsList(-1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);  
    }
}