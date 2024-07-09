using System.Diagnostics.CodeAnalysis;
using Colir.DAL.Tests.Interfaces;
using Colir.DAL.Tests.Utils;
using Colir.Exceptions;
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
        List<Reaction> expected = _dbContext.Reactions
                                  .Include(nameof(Reaction.Author))
                                  .Include(nameof(Reaction.Message))
                                  .ToList();

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
        Reaction expected = _dbContext.Reactions
                                      .Include(nameof(Reaction.Author))
                                      .Include(nameof(Reaction.Message))
                                      .First(r => r.Id == 1);

        // Act
        var result = await _reactionRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new ReactionEqualityComparer()));
        Assert.That(result.Author, Is.EqualTo(expected.Author).Using(new UserEqualityComparer()));
        Assert.That(result.Message, Is.EqualTo(expected.Message).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenReactionWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _reactionRepository.GetByIdAsync(100);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task GetReactionsOnMessage_ReturnsAllReactionsOnMessage()
    {
        // Arrange
        var expected = new List<Reaction>
        { 
            _dbContext.Reactions
                .Include(nameof(Reaction.Author))
                .Include(nameof(Reaction.Message))
                .FirstOrDefault(r => r.MessageId == 1)! 
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
    public async Task GetReactionsOnMessage_ThrowsNotFoundException_WhenMessageWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _reactionRepository.GetReactionsOnMessage(100);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
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
        _reactionRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Reactions.Count() == 2);
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenAuthorWasNotFound()
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
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task AddAsync_ThrowsArgumentException_WhenMessageWasNotFound()
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
        Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Test]
    public async Task Delete_DeletesReaction()
    {
        // Arrange
        var reactionToDelete = _dbContext.Reactions.First();

        // Act
        _reactionRepository.Delete(reactionToDelete);
        _reactionRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Reactions.Count() == 0);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenReactionDoesNotExist()
    {
        // Arrange
        var reactionToDelete = new Reaction() { Id = 404 };

        // Act
        TestDelegate act = () => _reactionRepository.Delete(reactionToDelete);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesReaction()
    {
        // Act
        await _reactionRepository.DeleteByIdAsync(1);
        _reactionRepository.SaveChanges();

        // Assert
        Assert.That(_dbContext.Reactions.Count() == 0);
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenReactionWasNotFoundById()
    {
        // Arrange
        var reactionToDelete = new Reaction() { Id = 404, Symbol = "😎" };

        // Act
        TestDelegate act = () => _reactionRepository.Delete(reactionToDelete);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenReactionWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _reactionRepository.DeleteByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesReaction()
    {
        // Arrange
        var reactionToUpdate = _dbContext.Reactions.First();
        reactionToUpdate.Symbol = "😎";

        // Act
        _reactionRepository.Update(reactionToUpdate);
        _reactionRepository.SaveChanges();
        var updatedReaction = _dbContext.Reactions.First(r => r.Id == reactionToUpdate.Id);

        // Assert
        Assert.That(updatedReaction.Symbol, Is.EqualTo("😎"));
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenReactionDoesNotExist()
    {
        // Arrange
        var reactionToUpdate = new Reaction() { Id = 404, Symbol = "😎" };

        // Act
        TestDelegate act = () => _reactionRepository.Update(reactionToUpdate);

        // Assert
        Assert.Throws<NotFoundException>(act);
    }
}