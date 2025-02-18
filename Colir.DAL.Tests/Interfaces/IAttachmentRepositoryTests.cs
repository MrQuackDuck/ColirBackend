﻿namespace Colir.DAL.Tests.Interfaces;

public interface IAttachmentRepositoryTests
{
    Task GetAllAsync_ReturnsAllAttachments();

    Task GetByIdAsync_ReturnsAttachment_WhenFound();
    Task GetByIdAsync_ThrowsAttachmentNotFoundException_WhenAttachmentWasNotFound();

    Task AddAsync_AddsNewAttachment();

    Task DeleteAsync_DeletesAttachment();
    Task DeleteAsync_ThrowsAttachmentNotFoundException_WhenAttachmentDoesNotExist();

    Task DeleteByIdAsync_DeletesAttachment();
    Task DeleteByIdAsync_ThrowsAttachmentNotFoundException_WhenAttachmentWasNotFoundById();

    Task DeleteAttachmentByPathAsync_DeletesAttachment();
    Task DeleteAttachmentByPathAsync_ThrowsAttachmentNotFoundException_WhenAttachmentWasNotFoundByFileName();

    Task Update_UpdatesAttachment();
    Task Update_ThrowsAttachmentNotFoundException_WhenAttachmentDoesNotExist();
}