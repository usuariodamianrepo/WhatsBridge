using Clone.DataAccess.Repositories.IRepository;
using Clone.Models;

namespace Clone.DataAccess.Repositories
{
    public class UserCredentialRepository : IUserCredentialRepository
    {
        private readonly IRepositoryAsync<UserCredential> _repository;

        public UserCredentialRepository(IRepositoryAsync<UserCredential> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
    }
}
