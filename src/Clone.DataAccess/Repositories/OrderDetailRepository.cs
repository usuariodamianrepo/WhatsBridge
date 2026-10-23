using Clone.DataAccess.Repositories.IRepository;
using Clone.Models;

namespace Clone.DataAccess.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly IRepositoryAsync<OrderDetail> _repository;

        public OrderDetailRepository(IRepositoryAsync<OrderDetail> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
    }
}
