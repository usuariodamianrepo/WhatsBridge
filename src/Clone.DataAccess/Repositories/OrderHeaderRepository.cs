using Clone.DataAccess.Repositories.IRepository;
using Clone.Models;

namespace Clone.DataAccess.Repositories
{
    public class OrderHeaderRepository : IOrderHeaderRepository
    {
        private readonly IRepositoryAsync<OrderHeader> _repository;

        public OrderHeaderRepository(IRepositoryAsync<OrderHeader> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
    }
}
