namespace Colir.DAL.Tests.Interfaces;

public interface IReactionRepositoryTests
{
    Task GetAllAsync_ReturnsAllReactions();

    Task GetByIdAsync_ReturnsReaction_WhenFound();
    Task GetByIdAsync_ThrowsNotFoundException_WhenReactionWasNotFound();

    Task GetReactionsOnMessage_ReturnsAllReactionsOnMessage();
    Task GetReactionsOnMessage_ThrowsNotFoundException_WhenMessageWasNotFound();

    Task AddAsync_AddsNewReaction();
    Task AddAsync_ThrowsArgumentException_WhenAuthorWasNotFound();
    Task AddAsync_ThrowsArgumentException_WhenMessageWasNotFound();

    Task Delete_DeletesReaction();
    Task Delete_ThrowsNotFoundException_WhenReactionDoesNotExist();

    Task DeleteByIdAsync_DeletesReaction();
    Task Delete_ThrowsNotFoundException_WhenReactionWasNotFoundById();

    Task Update_UpdatesReaction();
    Task Update_ThrowsNotFoundException_WhenReactionDoesNotExist();
}