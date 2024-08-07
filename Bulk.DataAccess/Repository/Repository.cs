using Bulk.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bulk.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;
    public Repository(ApplicationDbContext db)
    {
        _db = db;
        this.dbSet = _db.Set<T>();
        // _db.Categories == dbSet
        _db.Products.Include(u => u.Category);
    }
    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null) 
    {
        IQueryable<T> query = dbSet;
        if (filter is not null)
        {
            query = query.Where(filter);
        }        
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var prop in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop);
            }
        }
        return query.ToList();
    }

    public T Get(Expression<Func<T, bool>> filter, string includeProperties, bool tracked = false)
    {
        IQueryable<T> query;
        if (tracked)
        {
            query = dbSet;
        }
        else
        {
            query = dbSet.AsNoTracking();
        }
        
        query = query.Where(filter);
        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var prop in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(prop);
            }
        }
        return query.FirstOrDefault();
    }

    public void Add(T entity) => dbSet.Add(entity);

    public void Remove(T entity) => dbSet.Remove(entity);

    public void RemoveRange(IEnumerable<T> entity) => dbSet.RemoveRange(entity);
}