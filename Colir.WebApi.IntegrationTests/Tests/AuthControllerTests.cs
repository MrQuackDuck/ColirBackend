using System.IdentityModel.Tokens.Jwt;
using Colir.Communication.Enums;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Colir.WebApi.IntegrationTests.Tests;

public class AuthControllerTests
{
    private ColirWebAppFactory _application;
    private IConfiguration _configuration;
    private HttpClient _client;

    [OneTimeSetUp]
    public void Setup()
    {
        // Building the application
        _application = new ColirWebAppFactory();
        _configuration = _application.Services.GetRequiredService<IConfiguration>();
        _client = _application.CreateClient();
    }

    [OneTimeTearDown]
    public void CleanUp()
    {
        // Ensuring the database is deleted
        _application.DeleteDatabase();
    }

    [Test]
    public async Task AnonymousLogin_ReturnsValidJwt()
    {
        // Arrange
        var query = new Dictionary<string, string?>
        {
            ["name"] = "ColirUsername",
        };

        // Act
        var response = await _client.PostAsync(QueryHelpers.AddQueryString("API/Auth/AnonymousLogin", query), null);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var jwtToken = JObject.Parse(responseBody)["jwtToken"]!.Value<string>();
        JwtSecurityToken jwtTokenDecoded = (new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken)
            ?? throw new NullReferenceException();

        // Assert
        Assert.That(jwtTokenDecoded.Claims.FirstOrDefault(claim => claim.Type == "Id") != null);
        Assert.That(jwtTokenDecoded.Claims.FirstOrDefault(claim => claim.Type == "HexId") != null);
        Assert.That(jwtTokenDecoded.Claims.FirstOrDefault(claim => claim.Type == "AuthType") != null);
    }

    [Test]
    public async Task AnonymousLogin_ReturnsError_WhenNameTooShort()
    {
        // Arrange
        var minUsernameLength = int.Parse(_configuration["AppSettings:MinUsernameLength"]!);

        var query = new Dictionary<string, string?>();

        if (minUsernameLength <= 1)
            query["name"] = string.Empty;
        else
            query["name"] = new string('a', minUsernameLength - 1);

        // Act
        var response = await _client.PostAsync(QueryHelpers.AddQueryString("API/Auth/AnonymousLogin", query), null);

        dynamic responseBody = JObject.Parse(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.That(responseBody["errorCode"] == ErrorCode.StringWasTooShort);
    }

    [Test]
    public async Task AnonymousLogin_ReturnsError_WhenNameTooLong()
    {
        // Arrange
        var maxUsernameLength = int.Parse(_configuration["AppSettings:MaxUsernameLength"]!);

        var query = new Dictionary<string, string?>();

        query["name"] = new string('a', maxUsernameLength + 1);

        // Act
        var response = await _client.PostAsync(QueryHelpers.AddQueryString("API/Auth/AnonymousLogin", query), null);

        dynamic responseBody = JObject.Parse(await response.Content.ReadAsStringAsync());

        // Assert
        Assert.That(responseBody["errorCode"] == ErrorCode.StringWasTooLong);
    }
}