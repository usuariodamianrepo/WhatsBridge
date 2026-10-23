using Clone.Models;

namespace Clone.DataAccess.Repositories.IRepository
{
    public interface IRepositoryAsync<T> where T : AuditableEntity
    {
        IQueryable<T> Entities { get; }
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task RemoveAsync(T entity);
        Task UpdateAsync(T entity);
    }
}