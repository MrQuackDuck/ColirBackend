using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class AttachmentRepositoryTests : IAttachmentRepositoryTests
{
    [Test]
    public async Task GetAllAsync_ReturnsAllAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ReturnsAttachment_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task GetByIdAsync_ThrowsNotFoundException_WhenAttachmentWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_AddsNewAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ReturnsAddedAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task AddAsync_ThrowsNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_DeletesFile()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Delete_ThrowsNotFoundException_WhenAttachmentDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_DeletesFile()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task DeleteByIdAsync_ThrowsNotFoundException_WhenAttachmentWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_UpdatesAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public async Task Update_ThrowsNotFoundException_WhenAttachmentDoesNotExist()
    {
        throw new NotImplementedException();
    }
}