﻿namespace Colir.DAL.Tests.Interfaces;

public interface IUserRepositoryTests
{
    Task GetAllAsync_ReturnsAllUsers();

    Task GetByIdAsync_ReturnsUser_WhenFound();
    Task GetByIdAsync_ThrowsNotFoundException_WhenUserWasNotFound();

    Task GetByHexIdAsync_ReturnsUser_WhenFound();
    Task GetByHexIdAsync_ThrowsNotFoundException_WhenUserWasNotFound();
    Task GetByHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect();

    Task AddAsync_AddsNewUser();
    Task AddAsync_CreatesUserSettings();
    Task AddAsync_CreatesUserStatistics();
    Task AddAsync_AppliesJoinedRoomsToUser();
    Task AddAsync_ThrowsArgumentException_WhenHexAlreadyExists();
    Task AddAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect();
    Task AddAsync_ThrowsStringTooShortException_WhenUsernameTooShort();
    Task AddAsync_ThrowsStringTooLongException_WhenUsernameTooLong();
    Task AddAsync_ThrowsRoomNotFoundException_WhenOneOfJoinedRoomsWasNotFound();
    Task AddAsync_ThrowsRoomExpiredException_WhenOneOfJoinedRoomsIsExpired();

    Task Delete_DeletesUser();
    Task Delete_DeletesUserSettings();
    Task Delete_DeletesUserStatistics();
    Task Delete_ThrowsNotFoundException_WhenUserDoesNotExist();

    Task DeleteByIdAsync_DeletesUser();
    Task DeleteByIdAsync_DeletesUserSettings();
    Task DeleteByIdAsync_DeletesUserStatistics();
    Task DeleteByIdAsync_ThrowsNotFoundException_WhenUserWasNotFoundById();

    Task Update_UpdatesUser();
    Task Update_ThrowsStringTooLongException_WhenNameTooLong();
    Task Update_ThrowsStringTooShortException_WhenNameTooShort();
    Task Update_ThrowsNotFoundException_WhenUserDoesNotExist();
}