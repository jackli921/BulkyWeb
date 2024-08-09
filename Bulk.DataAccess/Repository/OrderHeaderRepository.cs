using Bulk.DataAccess.Data;
using Bulky.Models;
using BulkyWeb.Models;
using System.Linq.Expressions;

namespace Bulk.DataAccess.Repository;

public class OrderHeaderRepository: Repository<OrderHeader>, IOrderHeaderRepository
{
    private ApplicationDbContext _db;
    public OrderHeaderRepository(ApplicationDbContext db) : base(db) => _db = db;

    public void Update(OrderHeader obj) => _db.Update(obj);
}