using Clone.DataAccess.Data;
using Clone.DataAccess.Repositories.IRepository;
using Clone.Models;
using Clone.Models.Dtos;
using Clone.Utility;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Clone.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private Hashtable _repositories;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        public IRepositoryAsync<T> Repository<T>() where T : AuditableEntity
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(RepositoryAsync<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _db);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepositoryAsync<T>)_repositories[type];
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        #region Users
        public IQueryable<UserBySearchDto> UserBySearch(string id, string name, string phoneNumber)
        {
            var query = (
                from user in _db.Users
                join userRole in _db.UserRoles on user.Id equals userRole.UserId into userRoles
                select new UserBySearchDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    State = user.State,
                    PostalCode = user.PostalCode,
                    CompanyId = user.CompanyId,
                    CompanyName = user.Company != null ? user.Company.Name : null,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    LockoutEnd = user.LockoutEnd,
                    Roles = string.Join(", ",
                        from ur in userRoles
                        join role in _db.Roles on ur.RoleId equals role.Id
                        select role.Name)
                }).AsNoTracking();

            if(!string.IsNullOrEmpty(id))
            {
                query = query.Where(u => u.Id == id);
            }
            else
            {
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(u => u.Name!.Contains(name));
                }
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    query = query.Where(u => u.PhoneNumber!.Contains(phoneNumber));
                }
            }

            return query.Take(MaxRows.MaxFifty);
        }
        #endregion
    }
}
