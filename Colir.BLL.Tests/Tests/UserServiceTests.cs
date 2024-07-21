using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.User;
using Colir.BLL.Services;
using Colir.BLL.Tests.Interfaces;
using Colir.BLL.Tests.Utils;
using Colir.Exceptions;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.BLL.Tests.Tests;

public class UserServiceTests : IUserServiceTests
{
    private ColirDbContext _dbContext;
    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize the service
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(config => config["MinUsernameLength"]).Returns("2");
        configMock.Setup(config => config["MaxUsernameLength"]).Returns("50");

        var unitOfWork = new UnitOfWork(_dbContext, configMock.Object);
        var mapper = AutomapperProfile.InitializeAutoMapper().CreateMapper();

        var hexGeneratorMock = new Mock<IHexColorGenerator>();
        hexGeneratorMock.Setup(h => h.GetUniqueHexColor()).ReturnsAsync(0x123456);
        
        _userService = new UserService(unitOfWork, mapper, hexGeneratorMock.Object);

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
    public async Task AuthorizeWithGitHubAsync_CreatesUser()
    {
        // Arrange
        var request = new RequestToAuthorizeWithGitHub
        {
            GitHubId = "0000",
            HexId = 0xFFABCD,
            Username = "My User"
        };

        // Act
        await _userService.AuthorizeWithGitHubAsync(request);

        // Assert
        Assert.That(_dbContext.Users.Count() == 4);
    }

    [Test]
    public async Task AuthorizeWithGitHubAsync_ReturnsCorrectData()
    {
        // Arrange
        var request = new RequestToAuthorizeWithGitHub
        {
            GitHubId = "0000",
            HexId = 0xFFABCD,
            Username = "Fourth User"
        };

        // Act
        var result = await _userService.AuthorizeWithGitHubAsync(request);

        // Assert
        Assert.That(result.Username == request.Username);
        Assert.That(result.HexId == request.HexId);
        Assert.That(result.AuthType == DAL.Enums.UserAuthType.Github);
    }

    [Test]
    public async Task AuthorizeWithGitHubAsync_ThrowsArgumentException_WhenHexIsNotUnique()
    {
        // Arrange
        var request = new RequestToAuthorizeWithGitHub
        {
            GitHubId = "0000",
            HexId = 0xFFFFFF,
            Username = "Fourth User"
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.AuthorizeWithGitHubAsync(request);

        // Assert
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AuthorizeWithGitHubAsync_ThrowsStringTooShortException_WhenNewUsernameTooShort()
    {
        // Arrange
        var request = new RequestToAuthorizeWithGitHub
        {
            GitHubId = "0000",
            HexId = 0xFFABCD,
            Username = new string('a', 1)
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.AuthorizeWithGitHubAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooShortException>(act);
    }

    [Test]
    public async Task AuthorizeWithGitHubAsync_ThrowsStringTooLongException_WhenNewUsernameTooLong()
    {
        // Arrange
        var request = new RequestToAuthorizeWithGitHub
        {
            GitHubId = "0000",
            HexId = 0xFFABCD,
            Username = new string('a', 51)
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.AuthorizeWithGitHubAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooLongException>(act);
    }

    [Test]
    public async Task AuthorizeAsAnnoymousAsync_CreatesUser()
    {
        // Arrange
        var request = new RequestToAuthorizeAsAnnoymous
        {
            DesiredUsername = "Fourth User"
        };

        // Act
        await _userService.AuthorizeAsAnnoymousAsync(request);

        // Assert
        Assert.That(_dbContext.Users.Count() == 4);
    }

    [Test]
    public async Task AuthorizeAsAnnoymousAsync_ReturnsCorrectData()
    {
        // Arrange
        var request = new RequestToAuthorizeAsAnnoymous
        {
            DesiredUsername = "Fourth User"
        };

        // Act
        var result = await _userService.AuthorizeAsAnnoymousAsync(request);

        // Assert
        Assert.That(result.Username == request.DesiredUsername);
        Assert.That(result.AuthType == DAL.Enums.UserAuthType.Anonymous);
    }

    [Test]
    public async Task AuthorizeAsAnnoymousAsync_ThrowsStringTooShortException_WhenNewUsernameTooShort()
    {
        // Arrange
        var request = new RequestToAuthorizeAsAnnoymous
        {
            DesiredUsername = new string('a', 1)
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.AuthorizeAsAnnoymousAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooShortException>(act);
    }

    [Test]
    public async Task AuthorizeAsAnnoymousAsync_ThrowsStringTooLongException_WhenNewUsernameTooLong()
    {
        // Arrange
        var request = new RequestToAuthorizeAsAnnoymous
        {
            DesiredUsername = new string('a', 51)
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.AuthorizeAsAnnoymousAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooLongException>(act);
    }

    [Test]
    public async Task ChangeUsernameAsync_ChangesUsername()
    {
        // Arrange
        var request = new RequestToChangeUsername
        {
            IssuerId = 1,
            DesiredUsername = "User #1"
        };

        // Act
        await _userService.ChangeUsernameAsync(request);

        // Assert
        var userAfter = _dbContext.Users.First(u => u.Id == 1);
        Assert.That(request.DesiredUsername == userAfter.Username);
    }

    [Test]
    public async Task ChangeUsernameAsync_StringTooShortException_WhenNewUsernameTooShort()
    {
        // Arrange
        var request = new RequestToChangeUsername
        {
            IssuerId = 1,
            DesiredUsername = "1"
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.ChangeUsernameAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooShortException>(act);
    }

    [Test]
    public async Task ChangeUsernameAsync_ThrowsStringTooLongException_WhenNewUsernameTooLong()
    {
        // Arrange
        var request = new RequestToChangeUsername
        {
            IssuerId = 1,
            DesiredUsername = new string('a', 51)
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.ChangeUsernameAsync(request);

        // Assert
        Assert.ThrowsAsync<StringTooLongException>(act);
    }

    [Test]
    public async Task ChangeSettingsAsync_UpdatesSettings()
    {
        // Arrange
        var request = new RequestToChangeSettings
        {
            IssuerId = 1,
            NewSettings = new UserSettingsModel() { StatisticsEnabled = false }
        };

        // Act
        await _userService.ChangeSettingsAsync(request);

        // Assert
        var userAfter = _dbContext.Users.Include(nameof(User.UserSettings)).First(u => u.Id == 1);
        Assert.That(userAfter.UserSettings.StatisticsEnabled == false);
    }

    [Test]
    public async Task ChangeSettingsAsync_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrange
        var request = new RequestToChangeSettings
        {
            IssuerId = 404,
            NewSettings = new UserSettingsModel() { StatisticsEnabled = false }
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.ChangeSettingsAsync(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task DeleteAccount_DeletesAccount()
    {
        // Arrnge
        var userCountBefore = _dbContext.Users.Count();
        var request = new RequestToDeleteAccount
        {
            IssuerId = 1,
        };

        // Act
        await _userService.DeleteAccount(request);

        // Assert
        var userCountAfter = _dbContext.Users.Count();
        Assert.That(userCountBefore - userCountAfter == 1);
    }

    [Test]
    public async Task DeleteAccount_ThrowsUserNotFoundException_WhenIssuerWasNotFound()
    {
        // Arrnge
        var request = new RequestToDeleteAccount
        {
            IssuerId = 404,
        };

        // Act
        AsyncTestDelegate act = async () => await _userService.DeleteAccount(request);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }
}