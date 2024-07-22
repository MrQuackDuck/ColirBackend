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
public class AttachmentRepositoryTests : IAttachmentRepositoryTests
{
    private ColirDbContext _dbContext;
    private AttachmentRepository _attachmentRepository;
    
    [SetUp]
    public void SetUp()
    {
        // Create database context
        _dbContext = UnitTestHelper.CreateDbContext();
        
        // Initialize attachment repository
        _attachmentRepository = new AttachmentRepository(_dbContext);
        
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
    public async Task GetAllAsync_ReturnsAllAttachments()
    {
        // Arrange
        var expected = _dbContext.Attachments
                                 .Include(nameof(Attachment.Message))
                                 .ToList();
        
        // Act
        var result = await _attachmentRepository.GetAllAsync();
        
        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new AttachmentEqualityComparer()));
        
                
        Assert.That(result.Select(r => r.Message).OrderBy(r => r.Id),
            Is.EqualTo(expected.Select(r => r.Message).OrderBy(r => r.Id)).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ReturnsAttachment_WhenFound()
    {
        // Arrange
        var expected = _dbContext.Attachments
                                 .Include(nameof(Attachment.Message))
                                 .First(a => a.Id == 1);

        // Act
        var result = await _attachmentRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(expected).Using(new AttachmentEqualityComparer()));
        Assert.That(result.Message, Is.EqualTo(expected.Message).Using(new MessageEqualityComparer()));
    }

    [Test]
    public async Task GetByIdAsync_ThrowsAttachmentNotFoundException_WhenAttachmentWasNotFound()
    {
        // Act
        AsyncTestDelegate act = async () => await _attachmentRepository.GetByIdAsync(404);

        // Assert
        Assert.ThrowsAsync<AttachmentNotFoundException>(act);
    }

    [Test]
    public async Task AddAsync_AddsNewAttachment()
    {
        // Arrange
        var attachmentToAdd = new Attachment
        {
            Id = 2,
            Filename = "newFile.zip",
            Path = "/tests/newFile.zip",
            SizeInBytes = 100,
            MessageId = 2, // Message: "Reply to first message"
        };
        
        // Act
        await _attachmentRepository.AddAsync(attachmentToAdd);
        _attachmentRepository.SaveChanges();
        
        // Assert
        Assert.That(_dbContext.Attachments.Count() == 2);
    }

    [Test]
    public async Task AddAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound()
    {
        // Arrange
        var attachmentToAdd = new Attachment
        {
            Id = 2,
            Filename = "newFile.zip",
            Path = "/tests/newFile.zip",
            SizeInBytes = 100,
            MessageId = 404,
        };
        
        // Act
        AsyncTestDelegate act = async () => await _attachmentRepository.AddAsync(attachmentToAdd);
        
        // Assert
        Assert.ThrowsAsync<MessageNotFoundException>(act);   
    }

    [Test]
    public async Task Delete_DeletesAttachment()
    {
        // Arrange
        var attachmentToDelete = _dbContext.Attachments.AsNoTracking().First();
        
        // Act
        _attachmentRepository.Delete(attachmentToDelete);
        _attachmentRepository.SaveChanges();
        
        // Assert
        Assert.That(_dbContext.Attachments.Count() == 0);
    }

    [Test]
    public async Task Delete_ThrowsAttachmentNotFoundException_WhenAttachmentDoesNotExist()
    {
        // Arrange
        var attachmentToDelete = new Attachment
        {
            Id = 404,
            Filename = "newFile.zip",
            Path = "/tests/newFile.zip",
            SizeInBytes = 100,
            MessageId = 2, // Message: "Reply to first message"
        };
        
        // Act
        TestDelegate act = () => _attachmentRepository.Delete(attachmentToDelete);
        
        // Assert
        Assert.Throws<AttachmentNotFoundException>(act);
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAttachment()
    {
        // Act
        await _attachmentRepository.DeleteByIdAsync(1);
        _attachmentRepository.SaveChanges();
        
        // Assert
        Assert.That(_dbContext.Attachments.Count() == 0);
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsAttachmentNotFoundException_WhenAttachmentWasNotFoundById()
    {
        // Act
        AsyncTestDelegate act = async () => await _attachmentRepository.DeleteByIdAsync(404);
        
        // Assert
        Assert.ThrowsAsync<AttachmentNotFoundException>(act);
    }

    [Test]
    public async Task Update_UpdatesAttachment()
    {
        // Arrange
        var attachmentToUpdate = _dbContext.Attachments.AsNoTracking().First();
        
        // Act
        attachmentToUpdate.SizeInBytes = 100;
        _attachmentRepository.Update(attachmentToUpdate);
        _attachmentRepository.SaveChanges();
        
        // Assert
        Assert.That(_dbContext.Attachments.First().SizeInBytes == 100);
    }

    [Test]
    public async Task Update_ThrowsAttachmentNotFoundException_WhenAttachmentDoesNotExist()
    {
        // Arrange
        var attachmentToUpdate = new Attachment
        {
            Id = 404,
            Filename = "newFile.zip",
            Path = "/tests/newFile.zip",
            SizeInBytes = 100,
            MessageId = 2, // Message: "Reply to first message"
        };
        
        // Act
        TestDelegate act = () => _attachmentRepository.Update(attachmentToUpdate);
        
        // Assert
        Assert.Throws<AttachmentNotFoundException>(act);
    }
}