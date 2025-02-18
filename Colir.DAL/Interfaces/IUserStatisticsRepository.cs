﻿using DAL.Entities;

namespace DAL.Interfaces;

public interface IUserStatisticsRepository : IRepository<UserStatistics>
{
    Task<UserStatistics> GetByUserHexIdAsync(int hexId, string[]? overriddenIncludes = default);
}