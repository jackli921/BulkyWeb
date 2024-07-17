using Bulk.DataAccess.Data;
using Bulky.Models;

namespace Bulk.DataAccess.Repository;

public class ProductRepository: Repository<Product>, IProductRepository
{
    private ApplicationDbContext _db;
    public ProductRepository(ApplicationDbContext db) : base(db) => _db = db;

    public void Update(Product obj) => _db.Update(obj);
}