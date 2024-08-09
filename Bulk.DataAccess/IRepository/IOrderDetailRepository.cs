using Bulky.Models;
using BulkyWeb.Models;

namespace Bulk.DataAccess.Repository;

public interface IOrderDetailRepository: IRepository<OrderDetail>
{
    void Update(OrderDetail obj);
}