using System.Linq.Expressions;

namespace Bulk.DataAccess.Repository;

public interface IRepository<T> where T : class
{
    // T - Cateogry
    IEnumerable<T> GetAll(string? includeProperties = null);
    T Get(Expression<Func<T, bool>> filter,string? includeProperties = null);
    void Add(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entity);
}