using Singula.Core.Core.Entities;

namespace Singula.Core.Core.Interfaces;

public interface IReporteRepository : IRepository<Reporte>
{
    Task<IEnumerable<Reporte>> GetByUsuarioAsync(string usuario);
    Task<IEnumerable<Reporte>> GetByTipoAsync(TipoReporte tipo);
    Task<IEnumerable<Reporte>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<Reporte?> GetByRutaAsync(string ruta);
}
