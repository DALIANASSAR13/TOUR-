using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TourGuide.DAL.Data;
using TourGuide.DAL.Repositories.Interfaces;

namespace TourGuide.DAL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> Set;

    public Repository(AppDbContext context)
    {
        Context = context;
        Set = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await Set.FindAsync(new object[] { id }, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default) =>
        await Set.AddAsync(entity, ct);

    public void Update(T entity) => Set.Update(entity);

    public Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        Set.Where(predicate).ToListAsync(ct);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        Set.AnyAsync(predicate, ct);
}
