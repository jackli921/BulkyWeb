using Bulky.Models;
using BulkyWeb.Models;

namespace Bulk.DataAccess.Repository;

public interface ICategoryRepository: IRepository<Category>
{
    void Update(Category obj);
}