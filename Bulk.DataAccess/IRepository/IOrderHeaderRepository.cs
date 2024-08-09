using Bulky.Models;
using BulkyWeb.Models;

namespace Bulk.DataAccess.Repository;

public interface IOrderHeaderRepository: IRepository<OrderHeader>
{
    void Update(OrderHeader obj);
}