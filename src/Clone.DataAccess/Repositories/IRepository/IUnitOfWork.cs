using Clone.Models;
using Clone.Models.Dtos;

namespace Clone.DataAccess.Repositories.IRepository
{
    public interface IUnitOfWork
    {
        IRepositoryAsync<T> Repository<T>() where T : AuditableEntity;
        Task SaveAsync();
        IQueryable<UserBySearchDto> UserBySearch(string id, string name, string phoneNumber);
    }
}
