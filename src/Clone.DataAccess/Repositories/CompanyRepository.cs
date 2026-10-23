using Clone.DataAccess.Repositories.IRepository;
using Clone.Models;

namespace Clone.DataAccess.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IRepositoryAsync<Company> _repository;

        public CompanyRepository(IRepositoryAsync<Company> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
    }
}
