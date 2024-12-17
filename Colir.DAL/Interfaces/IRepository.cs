namespace DAL.Interfaces;

public interface IRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAsync(string[]? overriddenIncludes = default);
    Task<TEntity> GetByIdAsync(long id, string[]? overriddenIncludes = default);
    Task AddAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task DeleteByIdAsync(long id);
    void Update(TEntity entity);
    void SaveChanges();
    Task SaveChangesAsync();
}