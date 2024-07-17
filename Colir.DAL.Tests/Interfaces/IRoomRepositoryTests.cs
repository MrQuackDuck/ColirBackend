namespace Colir.DAL.Tests.Interfaces;

public interface IRoomRepositoryTests
{
    Task GetAllAsync_ReturnsAllRooms();

    Task GetByIdAsync_ReturnsRoom_WhenFound(long id);
    Task GetByIdAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFound(long id);

    Task AddAsync_AddsNewRoom();
    Task AddAsync_AppliesJoinedUsersToRoom();
    Task AddAsync_ThrowsStringTooLongException_WhenNameTooLong();
    Task AddAsync_ThrowsStringTooShortException_WhenNameTooShort();
    Task AddAsync_ThrowsRoomExpiredException_WhenWrongExpiryDateWasProvided();
    Task AddAsync_ThrowsUserNotFoundException_WhenOwnerWasNotFound();

    Task Delete_DeletesRoom();
    Task Delete_DeletesAllRelatedAttachments();
    Task Delete_DeletesAllRelatedMessages();
    Task Delete_DeletesAllRelatedReactions();
    Task Delete_ThrowsRoomNotFoundException_WhenRoomDoesNotExist();

    Task DeleteByIdAsync_DeletesRoom();
    Task DeleteByIdAsync_DeletesAllRelatedAttachments();
    Task DeleteByIdAsync_DeletesAllRelatedMessages();
    Task DeleteByIdAsync_DeletesAllRelatedReactions();
    Task DeleteByIdAsync_ThrowsRoomNotFoundException_WhenRoomWasNotFoundById();

    Task Update_UpdatesRoom();
    Task Update_ThrowsStringTooLongException_WhenNameTooLong();
    Task Update_ThrowsStringTooShortException_WhenNameTooShort();
    Task Update_ThrowsRoomNotFoundException_WhenRoomDoesNotExist();
    
    Task DeleteAllExpiredAsync_DeletesAllExpiredRooms();
    Task DeleteAllExpiredAsync_ThrowsRoomNotFoundException_WhenNoExpiredRoomsExist();
}