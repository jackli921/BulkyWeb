using Bulk.DataAccess.Data;
using Bulky.Models;
using BulkyWeb.Models;
using System.Linq.Expressions;

namespace Bulk.DataAccess.Repository;

public class OrderDetailRepository: Repository<OrderDetail>, IOrderDetailRepository
{
    private ApplicationDbContext _db;
    public OrderDetailRepository(ApplicationDbContext db) : base(db) => _db = db;

    public void Update(OrderDetail obj) => _db.Update(obj);
}