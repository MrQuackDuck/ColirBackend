using DAL.Entities;

namespace DAL.Interfaces;

public interface IUserSettingsRepository : IRepository<UserSettings>
{
    Task<UserSettings> GetByUserHexIdAsync(string hexId);
}