using Microsoft.EntityFrameworkCore;
using Singula.Core.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;

        public EfRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _db.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            try
            {
                _db.Set<T>().Add(entity);
                await _db.SaveChangesAsync();
                return entity;
            }
            catch (DbUpdateException ex)
            {
                // Loggear la excepción completa con detalles
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"Error al guardar entidad en BD: {innerMsg}", ex);
            }
        }

        public virtual async Task<T?> UpdateAsync(T entity)
        {
            _db.Set<T>().Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            var entity = await _db.Set<T>().FindAsync(id);
            if (entity == null) return false;
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
