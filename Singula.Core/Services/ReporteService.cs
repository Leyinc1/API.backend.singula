using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IRepository<Reporte> _repo;

        public ReporteService(IRepository<Reporte> repo)
        {
            _repo = repo;
        }

        public async Task<ReporteDto> CreateAsync(ReporteDto dto)
        {
            var entity = new Reporte {
                TipoReporte = dto.TipoReporte,
                Formato = dto.Formato,
                FiltrosJson = dto.FiltrosJson,
                GeneradoPor = dto.GeneradoPor,
                FechaGeneracion = DateTime.UtcNow, // UTC para PostgreSQL timestamptz
                RutaArchivo = dto.NombreArchivo ?? dto.RutaArchivo // Usar nombreArchivo si está presente, sino rutaArchivo (compatibilidad)
            };
            var created = await _repo.CreateAsync(entity);
            return new ReporteDto {
                IdReporte = created.IdReporte,
                TipoReporte = created.TipoReporte,
                Formato = created.Formato,
                FiltrosJson = created.FiltrosJson,
                GeneradoPor = created.GeneradoPor,
                FechaGeneracion = created.FechaGeneracion,
                RutaArchivo = created.RutaArchivo,
                IdSolicituds = created.IdSolicituds?.Select(s => s.IdSolicitud).ToList()
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<ReporteDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(r => new ReporteDto {
                IdReporte = r.IdReporte,
                TipoReporte = r.TipoReporte,
                Formato = r.Formato,
                FiltrosJson = r.FiltrosJson,
                GeneradoPor = r.GeneradoPor,
                FechaGeneracion = r.FechaGeneracion,
                RutaArchivo = r.RutaArchivo,
                IdSolicituds = r.IdSolicituds?.Select(s => s.IdSolicitud).ToList()
            });
        }

        public async Task<ReporteDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new ReporteDto {
                IdReporte = e.IdReporte,
                TipoReporte = e.TipoReporte,
                Formato = e.Formato,
                FiltrosJson = e.FiltrosJson,
                GeneradoPor = e.GeneradoPor,
                FechaGeneracion = e.FechaGeneracion,
                RutaArchivo = e.RutaArchivo,
                IdSolicituds = e.IdSolicituds?.Select(s => s.IdSolicitud).ToList()
            };
        }

        public async Task<ReporteDto?> UpdateAsync(int id, ReporteDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.TipoReporte = dto.TipoReporte;
            e.Formato = dto.Formato;
            e.FiltrosJson = dto.FiltrosJson;
            e.GeneradoPor = dto.GeneradoPor;
            e.RutaArchivo = dto.RutaArchivo;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
