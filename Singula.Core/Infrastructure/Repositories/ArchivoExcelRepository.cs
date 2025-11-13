using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;
using Singula.Core.Core.Interfaces;
using Singula.Core.Infrastructure.Data;

namespace Singula.Core.Infrastructure.Repositories;

public class ArchivoExcelRepository : Repository<ArchivoExcel>, IArchivoExcelRepository
{
    public ArchivoExcelRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<ArchivoExcel>> GetByUsuarioAsync(string usuario)
    {
        return await _dbSet
            .Where(x => x.UsuarioCarga == usuario)
            .OrderByDescending(x => x.FechaCarga)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<ArchivoExcel>> GetByEstadoAsync(EstadoProcesamiento estado)
    {
        return await _dbSet
            .Where(x => x.Estado == estado)
            .OrderByDescending(x => x.FechaCarga)
            .ToListAsync();
    }
    
    public async Task<ArchivoExcel?> GetWithRegistrosAsync(int id)
    {
        return await _dbSet
            .Include(x => x.Registros)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<IEnumerable<ArchivoExcel>> GetRecentesAsync(int cantidad = 10)
    {
        return await _dbSet
            .OrderByDescending(x => x.FechaCarga)
            .Take(cantidad)
            .ToListAsync();
    }
}
