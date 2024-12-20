﻿namespace Colir.DAL.Tests.Interfaces;

public interface IUserStatisticsRepositoryTests
{
    Task GetAllAsync_ReturnsAllUsersStatistics();

    Task GetByUserHexIdAsync_ReturnsUserStatistics();
    Task GetByUserHexIdAsync_ThrowsUserNotFoundException_WhenUserWasNotFound();
    Task GetByUserHexIdAsync_ThrowsArgumentException_WhenHexFormatIsNotCorrect();

    Task GetByIdAsync_ReturnsUserStatistics_WhenFound();
    Task GetByIdAsync_ThrowsNotFoundException_WhenUserStatisticsWasNotFound();

    Task AddAsync_AddsNewUserStatistics();
    Task AddAsync_ThrowsArgumentException_WhenUserStatisticsAlreadyExist();
    Task AddAsync_ThrowsUserNotFoundException_WhenUserWasNotFound();

    Task DeleteAsync_DeletesUserStatistics();
    Task DeleteAsync_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist();

    Task DeleteByIdAsync_DeletesUserStatistics();
    Task DeleteByIdAsync_ThrowsNotFoundException_WhenUserStatisticsWasNotFoundById();

    Task Update_UpdatesUserStatistics();
    Task Update_ThrowsArgumentException_WhenProvidedAnotherUserId();
    Task Update_ThrowsNotFoundException_WhenUserStatisticsDoesNotExist();
}