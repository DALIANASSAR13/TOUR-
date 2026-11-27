using System.Linq.Expressions;

namespace TourGuide.DAL.Repositories.Interfaces;

// Generic data access. No Delete: nothing is deleted in the MVP. Never calls SaveChanges (Unit of Work does).
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
}
