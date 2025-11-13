using System.Linq.Expressions;

namespace Singula.Core.Core.Interfaces;

/// <summary>
/// Interfaz genérica de repositorio con operaciones CRUD
/// </summary>
public interface IRepository<T> where T : class
{
    // READ
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    
    // Paginación
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true);
    
    // CREATE
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    
    // UPDATE
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);
    
    // DELETE
    Task DeleteAsync(T entity);
    Task DeleteAsync(int id);
    Task DeleteRangeAsync(IEnumerable<T> entities);
}
