using Clone.DataAccess.Data;
using Clone.DataAccess.Repositories.IRepository;
using Clone.Models;
using Microsoft.EntityFrameworkCore;

namespace Clone.DataAccess.Repositories
{
    public class RepositoryAsync<T> : IRepositoryAsync<T> where T : AuditableEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public RepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();

        public async Task AddAsync(T entity) => await _dbContext.Set<T>().AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) => await _dbContext.Set<T>().AddRangeAsync(entities);

        public async Task<List<T>> GetAllAsync() => await _dbContext.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _dbContext.Set<T>().FindAsync(id);

        public async Task<List<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _dbContext
                .Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task RemoveAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            var set = _dbContext.Set<T>();
            var exist = await set.FindAsync(entity.Id).ConfigureAwait(false);
            if (exist == null)
            {
                throw new InvalidOperationException($"Entity of type {typeof(T).Name} with Id '{entity.Id}' was not found.");
            }
            var entry = _dbContext.Entry(exist);
            entry.CurrentValues.SetValues(entity);

            // Use the version the client had when opening the form
            entry.Property(e => e.xmin).OriginalValue = entity.xmin;
        }
    }
}