using Microsoft.EntityFrameworkCore;

namespace Template.Domain.Repository;

public class Repository(DbContext context) : IRepository
{
    public void Dispose()
    {
        context?.Dispose();
    }
    
    public IQueryable<T> Query<T>() where T : class
    {
        return context.Set<T>();
    }

    public void Add<T>(T entity) where T : class
    {
        context?.Set<T>().Add(entity);
    }

    public void Update<T>(T entity) where T : class
    {
        context?.Set<T>().Update(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
        context?.Set<T>().Remove(entity);
    }

    public IQueryable<T> QueryAsNoTracking<T>() where T : class
    {
        return context.Set<T>().AsNoTracking();
    }

    public Task<int> SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }
}