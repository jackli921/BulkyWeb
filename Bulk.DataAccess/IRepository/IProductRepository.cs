using Bulky.Models;

namespace Bulk.DataAccess.Repository;

public interface IProductRepository: IRepository<Product>
{
    void Update(Product obj);
}