using Bulky.Models;
using BulkyWeb.Models;

namespace Bulk.DataAccess.Repository;

public interface ICompanyRepository: IRepository<Company>
{
    void Update(Company obj);
}