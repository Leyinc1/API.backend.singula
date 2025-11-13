using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class ConfigSlaService : IConfigSlaService
    {
        private readonly IRepository<ConfigSla> _repo;

        public ConfigSlaService(IRepository<ConfigSla> repo)
        {
            _repo = repo;
        }

        public async Task<ConfigSlaDto> CreateAsync(ConfigSlaDto dto)
        {
            var entity = new ConfigSla {
                CodigoSla = dto.CodigoSla,
                Descripcion = dto.Descripcion,
                DiasUmbral = dto.DiasUmbral,
                EsActivo = dto.EsActivo,
                IdTipoSolicitud = dto.IdTipoSolicitud,
                CreadoPor = dto.CreadoPor
            };
            var created = await _repo.CreateAsync(entity);
            return new ConfigSlaDto {
                IdSla = created.IdSla,
                CodigoSla = created.CodigoSla,
                Descripcion = created.Descripcion,
                DiasUmbral = created.DiasUmbral,
                EsActivo = created.EsActivo,
                IdTipoSolicitud = created.IdTipoSolicitud,
                CreadoEn = created.CreadoEn,
                ActualizadoEn = created.ActualizadoEn,
                CreadoPor = created.CreadoPor,
                ActualizadoPor = created.ActualizadoPor
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return false;
            e.EsActivo = false;
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<IEnumerable<ConfigSlaDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(c => new ConfigSlaDto {
                IdSla = c.IdSla,
                CodigoSla = c.CodigoSla,
                Descripcion = c.Descripcion,
                DiasUmbral = c.DiasUmbral,
                EsActivo = c.EsActivo,
                IdTipoSolicitud = c.IdTipoSolicitud,
                CreadoEn = c.CreadoEn,
                ActualizadoEn = c.ActualizadoEn,
                CreadoPor = c.CreadoPor,
                ActualizadoPor = c.ActualizadoPor
            });
        }

        public async Task<ConfigSlaDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new ConfigSlaDto {
                IdSla = e.IdSla,
                CodigoSla = e.CodigoSla,
                Descripcion = e.Descripcion,
                DiasUmbral = e.DiasUmbral,
                EsActivo = e.EsActivo,
                IdTipoSolicitud = e.IdTipoSolicitud,
                CreadoEn = e.CreadoEn,
                ActualizadoEn = e.ActualizadoEn,
                CreadoPor = e.CreadoPor,
                ActualizadoPor = e.ActualizadoPor
            };
        }

        public async Task<ConfigSlaDto?> UpdateAsync(int id, ConfigSlaDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.CodigoSla = dto.CodigoSla;
            e.Descripcion = dto.Descripcion;
            e.DiasUmbral = dto.DiasUmbral;
            e.EsActivo = dto.EsActivo;
            e.IdTipoSolicitud = dto.IdTipoSolicitud;
            e.ActualizadoPor = dto.ActualizadoPor;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
