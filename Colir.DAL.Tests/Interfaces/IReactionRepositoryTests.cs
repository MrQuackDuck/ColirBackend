namespace Colir.DAL.Tests.Interfaces;

public interface IReactionRepositoryTests
{
    Task GetAllAsync_ReturnsAllReactions();

    Task GetByIdAsync_ReturnsReaction_WhenFound();
    Task GetByIdAsync_ThrowsReactionNotFoundException_WhenReactionWasNotFound();

    Task GetReactionsOnMessage_ReturnsAllReactionsOnMessage();
    Task GetReactionsOnMessage_ThrowsMessageNotFoundException_WhenMessageWasNotFound();

    Task AddAsync_AddsNewReaction();
    Task AddAsync_ThrowsUserNotFoundException_WhenAuthorWasNotFound();
    Task AddAsync_ThrowsMessageNotFoundException_WhenMessageWasNotFound();

    Task Delete_DeletesReaction();
    Task Delete_ThrowsReactionNotFoundException_WhenReactionDoesNotExist();

    Task DeleteByIdAsync_DeletesReaction();
    Task Delete_ThrowsReactionNotFoundException_WhenReactionWasNotFoundById();

    Task Update_UpdatesReaction();
    Task Update_ThrowsReactionNotFoundException_WhenReactionDoesNotExist();
}