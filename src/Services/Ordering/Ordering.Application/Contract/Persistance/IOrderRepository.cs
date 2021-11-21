using Ordering.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ordering.Application.Contract.Persistance
{
    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
    }
}
