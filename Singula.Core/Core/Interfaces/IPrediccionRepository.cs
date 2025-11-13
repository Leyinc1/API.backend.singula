using Singula.Core.Core.Entities;

namespace Singula.Core.Core.Interfaces;

public interface IPrediccionRepository : IRepository<Prediccion>
{
    Task<IEnumerable<Prediccion>> GetByIndicadorAsync(string indicador);
    Task<Prediccion?> GetUltimaPrediccionAsync(string indicador);
    Task<IEnumerable<Prediccion>> GetPrediccionesFuturasAsync(string indicador);
}
