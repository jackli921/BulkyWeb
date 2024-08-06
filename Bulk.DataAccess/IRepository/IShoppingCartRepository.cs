using Bulky.Models;
using BulkyWeb.Models;

namespace Bulk.DataAccess.Repository;

public interface IShoppingCartRepository: IRepository<ShoppingCart>
{
    void Update(ShoppingCart obj);
}