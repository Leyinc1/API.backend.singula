using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;
using Singula.Core.Core.Interfaces;
using Singula.Core.Infrastructure.Data;

namespace Singula.Core.Infrastructure.Repositories;

public class AlertaRepository : Repository<Alerta>, IAlertaRepository
{
    public AlertaRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Alerta>> GetNoLeidasAsync()
    {
        return await _dbSet
            .Where(x => !x.Leida)
            .OrderByDescending(x => x.FechaGeneracion)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Alerta>> GetByNivelAsync(NivelSeveridad nivel)
    {
        return await _dbSet
            .Where(x => x.Nivel == nivel)
            .OrderByDescending(x => x.FechaGeneracion)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Alerta>> GetByTipoAsync(TipoAlerta tipo)
    {
        return await _dbSet
            .Where(x => x.Tipo == tipo)
            .OrderByDescending(x => x.FechaGeneracion)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Alerta>> GetPendientesEnvioAsync()
    {
        return await _dbSet
            .Where(x => !x.EnviadaPorEmail)
            .OrderBy(x => x.FechaGeneracion)
            .ToListAsync();
    }
    
    public async Task MarcarComoLeidaAsync(int id)
    {
        var alerta = await GetByIdAsync(id);
        if (alerta != null)
        {
            alerta.Leida = true;
            await UpdateAsync(alerta);
        }
    }
    
    public async Task MarcarComoEnviadaAsync(int id)
    {
        var alerta = await GetByIdAsync(id);
        if (alerta != null)
        {
            alerta.EnviadaPorEmail = true;
            alerta.FechaEnvioEmail = DateTime.UtcNow;
            await UpdateAsync(alerta);
        }
    }
    
    public async Task<int> GetCountNoLeidasAsync()
    {
        return await _dbSet.CountAsync(x => !x.Leida);
    }
}

public class ReporteRepository : Repository<Reporte>, IReporteRepository
{
    public ReporteRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Reporte>> GetByUsuarioAsync(string usuario)
    {
        return await _dbSet
            .Where(x => x.UsuarioGenerador == usuario)
            .OrderByDescending(x => x.FechaGeneracion)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Reporte>> GetByTipoAsync(TipoReporte tipo)
    {
        return await _dbSet
            .Where(x => x.Tipo == tipo)
            .OrderByDescending(x => x.FechaGeneracion)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Reporte>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _dbSet
            .Where(x => x.FechaGeneracion >= fechaInicio && x.FechaGeneracion <= fechaFin)
            .OrderByDescending(x => x.FechaGeneracion)
            .ToListAsync();
    }
    
    public async Task<Reporte?> GetByRutaAsync(string ruta)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.RutaArchivoPDF == ruta);
    }
}

public class PrediccionRepository : Repository<Prediccion>, IPrediccionRepository
{
    public PrediccionRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Prediccion>> GetByIndicadorAsync(string indicador)
    {
        return await _dbSet
            .Where(x => x.Indicador == indicador)
            .OrderByDescending(x => x.FechaPrediccion)
            .ToListAsync();
    }
    
    public async Task<Prediccion?> GetUltimaPrediccionAsync(string indicador)
    {
        return await _dbSet
            .Where(x => x.Indicador == indicador)
            .OrderByDescending(x => x.FechaGeneracion)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<Prediccion>> GetPrediccionesFuturasAsync(string indicador)
    {
        return await _dbSet
            .Where(x => x.Indicador == indicador && x.FechaPrediccion > DateTime.UtcNow)
            .OrderBy(x => x.FechaPrediccion)
            .ToListAsync();
    }
}
