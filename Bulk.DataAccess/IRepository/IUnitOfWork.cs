namespace Bulk.DataAccess.Repository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    void Save();
}