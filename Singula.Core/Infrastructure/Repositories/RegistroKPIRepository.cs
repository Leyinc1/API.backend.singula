using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;
using Singula.Core.Core.Interfaces;
using Singula.Core.Infrastructure.Data;

namespace Singula.Core.Infrastructure.Repositories;

public class RegistroKPIRepository : Repository<RegistroKPI>, IRegistroKPIRepository
{
    public RegistroKPIRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<RegistroKPI>> GetByIndicadorAsync(string indicador)
    {
        return await _dbSet
            .Where(x => x.Indicador == indicador)
            .OrderByDescending(x => x.FechaMedicion)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<RegistroKPI>> GetByAreaAsync(string area)
    {
        return await _dbSet
            .Where(x => x.Area == area)
            .OrderByDescending(x => x.FechaMedicion)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<RegistroKPI>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _dbSet
            .Where(x => x.FechaMedicion >= fechaInicio && x.FechaMedicion <= fechaFin)
            .OrderBy(x => x.FechaMedicion)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<RegistroKPI>> GetByArchivoAsync(int archivoId)
    {
        return await _dbSet
            .Where(x => x.ArchivoExcelId == archivoId)
            .ToListAsync();
    }
    
    public async Task<RegistroKPI?> GetWithAlertasAsync(int id)
    {
        return await _dbSet
            .Include(x => x.Alertas)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<IEnumerable<string>> GetIndicadoresUnicosAsync()
    {
        return await _dbSet
            .Select(x => x.Indicador)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<string>> GetAreasUnicasAsync()
    {
        return await _dbSet
            .Where(x => x.Area != null)
            .Select(x => x.Area!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
    }
    
    public async Task<decimal> GetPromedioAsync(string indicador, DateTime? desde = null, DateTime? hasta = null)
    {
        IQueryable<RegistroKPI> query = _dbSet.Where(x => x.Indicador == indicador);
        
        if (desde.HasValue)
            query = query.Where(x => x.FechaMedicion >= desde.Value);
            
        if (hasta.HasValue)
            query = query.Where(x => x.FechaMedicion <= hasta.Value);
            
        return await query.AverageAsync(x => x.ValorActual);
    }
}
