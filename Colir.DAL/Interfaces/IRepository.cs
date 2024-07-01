namespace DAL.Interfaces;

public interface IRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(long id);
    Task AddAsync(TEntity entity);
    void Delete(TEntity entity);
    Task DeleteByIdAsync(long id);
    void Update(TEntity entity);
    void SaveChanges();
}