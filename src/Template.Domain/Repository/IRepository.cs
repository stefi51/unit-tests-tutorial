namespace Template.Domain.Repository;

public interface IRepository : IDisposable
{
    IQueryable<T> Query<T>() where T : class;
    void Add<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    IQueryable<T> QueryAsNoTracking<T>() where T : class;
    Task<int> SaveChangesAsync();
}