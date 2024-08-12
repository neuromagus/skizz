using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class GenericRepository<T>(StoreContext context) : IGenericRepository<T> where T : BaseEntity
{
    public void Add(T entity)
    {
        context.Set<T>().Add(entity);
    }

    public void Remove(T entity)
    {
        context.Set<T>().Remove(entity);
    }

    public void Update(T entity)
    {
        context.Set<T>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
    }

    public async Task<T?> GetByIdAsync(int id) => 
        await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IReadOnlyList<T>> ListAllAsync() => await context.Set<T>().AsNoTracking().ToListAsync();

    public async Task<T?> GetEntityWithSpec(ISpecification<T> spec) =>
        await ApplySpecification(spec).AsNoTracking().FirstOrDefaultAsync();

    public async Task<IReadOnlyList<T?>> ListAsync(ISpecification<T> spec) => 
        await ApplySpecification(spec).AsNoTracking().ToListAsync();

    public async Task<bool> SaveAllAsync() => await context.SaveChangesAsync() > 0;
    
    public bool Exists(int id) => context.Set<T>().Any(x => x.Id == id);

    private IQueryable<T> ApplySpecification(ISpecification<T> spec) =>
        SpecificationEvaluator<T>.GetQuery(context.Set<T>().AsQueryable(), spec);
}
