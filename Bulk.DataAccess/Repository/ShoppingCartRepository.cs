using Bulk.DataAccess.Data;
using Bulky.Models;
using BulkyWeb.Models;
using System.Linq.Expressions;

namespace Bulk.DataAccess.Repository;

public class ShoppingCartRepository: Repository<ShoppingCart>, IShoppingCartRepository
{
    private ApplicationDbContext _db;
    public ShoppingCartRepository(ApplicationDbContext db) : base(db) => _db = db;

    public void Update(ShoppingCart obj) => _db.Update(obj);
}