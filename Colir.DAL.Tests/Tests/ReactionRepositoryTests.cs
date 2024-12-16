using System.Diagnostics.CodeAnalysis;
using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions.NotFound;
using DAL;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Colir.DAL.Tests.Tests;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public class ReactionRepositoryTests : IReactionRepositoryTests
{
    private ColirDbContext _dbContext = default!;
    private ReactionRepository _reactionRepository = default!;

    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();

        // Initialize reaction repository
        _reactionRepository = new ReactionRepository(_dbContext);

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
    public async Task GetAllAsync_ReturnsAllReactions()
    {
        // Arrange
        List<Reaction> expected = await _dbContext.Reactions
            .Include(nameof(Reaction.Author))
            .Include(nameof(Reaction.Message))
            .ToListAsync();

        // Act
        var result = await _reactionRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.That(result, Is.EqualTo(expected).Using(new ReactionEqualityComparer()));

        Assert.That(result.Select(r => r.Author).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.Author).OrderBy(r => r.Id)).Using(new UserEqualityComparer()));

        Assert.That(result.Select(r => r.Message).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.Message).OrderBy(r => r.Id)).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsReaction_WhenFound()
    {
        // Arrange
        Reaction expected = await _dbContext.Reactions
            .Include(nameof(Reaction.Author))
            .Include(nameof(Reaction.Message))
            .FirstAsync(r => r.Id == 1);

        // Act
        var result = await _reactionRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new ReactionEqualityComparer()));
        Assert.That(result.Author, Is.EqualTo(expected.Author).Using(new UserEqualityComparer()));
        Assert.That(result.Message, Is.EqualTo(expected.Message).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsReactionNotFoundException_WhenReactionWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _reactionRepository.GetByIdAsync(100);

        // Assert
        Assert.ThrowsAsync<ReactionNotFoundException>(act);
    }

    [Test]
    public async Task GetReactionsOnMessage_ReturnsAllReactionsOnMessage()
    {
        // Arrange
        var expected = new List<Reaction>
        {
            (await _dbContext.Reactions
                .Include(nameof(Reaction.Author))
                .Include(nameof(Reaction.Message))
                .FirstOrDefaultAsync(r => r.MessageId == 1))!
        };

        // Act
        var result = await _reactionRepository.GetReactionsOnMessage(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new ReactionEqualityComparer()));

        Assert.That(result.Select(r => r.Author).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.Author).OrderBy(r => r.Id)).Using(new UserEqualityComparer()));

        Assert.That(result.Select(r => r.Message).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.Message).OrderBy(r => r.Id)).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetReactionsOnMessage_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _reactionRepository.GetReactionsOnMessage(100);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewReaction()
    {
        // Arrange
        var reactionToAdd = new Reaction()
        {
            Id = 2,
            Symbol = "😊",
            AuthorId = 1,
            MessageId = 1
        };

        // Act
        await _reactionRepository.AddAsync(reactionToAdd);
        await _reactionRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Reactions.CountAsync() == 2);
    }

    [Test]
    public async Task AddAsync_ThrowsUserNotFoundException_WhenAuthorWasNotFound()
    {
        // Arrange
        var reactionToAdd = new Reaction()
        {
            Id = 2,
            Symbol = "😊",
            AuthorId = 404,
            MessageId = 1
        };

        // Act
        AsyncTestDelegate act = async () => await _reactionRepository.AddAsync(reactionToAdd);

        // Assert
        Assert.ThrowsAsync<UserNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Arrange
        var reactionToAdd = new Reaction()
        {
            Id = 2,
            Symbol = "😊",
            AuthorId = 1,
            MessageId = 404
        };

        // Act
        AsyncTestDelegate act = async () => await _reactionRepository.AddAsync(reactionToAdd);

        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);
    }

    [Test]
    public async Task Delete_DeletesReaction()
    {
        // Arrange
        var reactionToDelete = await _dbContext.Reactions.AsNoTracking().FirstAsync();

        // Act
        _reactionRepository.Delete(reactionToDelete);
        await _reactionRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Reactions.CountAsync() == 0);
    }

    [Test]
    public async Task Delete_ThrowsReactionNotFoundException_WhenReactionDoesNotExist()
    {
        // Arrange
        var reactionToDelete = new Reaction() { Id = 404 };

        // Act
        TestDelegate act = () => _reactionRepository.Delete(reactionToDelete);

        // Assert
        Assert.Throws<ReactionNotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesReaction()
    {
        // Act
        await _reactionRepository.DeleteByIdAsync(1);
        await _reactionRepository.SaveChangesAsync();

        // Assert
        Assert.That(await _dbContext.Reactions.CountAsync() == 0);
    }

    [Test]
    public async Task Delete_ThrowsReactionNotFoundException_WhenReactionWasNotFoundById()
    {
        // Arrange
        var reactionToDelete = new Reaction() { Id = 404, Symbol = "😎" };

        // Act
        TestDelegate act = () => _reactionRepository.Delete(reactionToDelete);

        // Assert
        Assert.Throws<ReactionNotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsReactionNotFoundException_WhenReactionWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _reactionRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<ReactionNotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesReaction()
    {
        // Arrange
        var reactionToUpdate = await _dbContext.Reactions.AsNoTracking().FirstAsync();
        reactionToUpdate.Symbol = "😎";

        // Act
        _reactionRepository.Update(reactionToUpdate);
        await _reactionRepository.SaveChangesAsync();
        var updatedReaction = await _dbContext.Reactions.FirstAsync(r => r.Id == reactionToUpdate.Id);

        // Assert
        Assert.That(updatedReaction.Symbol, Is.EqualTo("😎"));
    }

    [Test]
    public async Task Update_ThrowsReactionNotFoundException_WhenReactionDoesNotExist()
    {
        // Arrange
        var reactionToUpdate = new Reaction() { Id = 404, Symbol = "😎" };

        // Act
        TestDelegate act = () => _reactionRepository.Update(reactionToUpdate);

        // Assert
        Assert.Throws<ReactionNotFoundException>(act);
    }
}