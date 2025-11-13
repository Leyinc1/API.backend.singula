using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class RolRegistroService : IRolRegistroService
    {
        private readonly IRepository<RolRegistro> _repo;

        public RolRegistroService(IRepository<RolRegistro> repo)
        {
            _repo = repo;
        }

        public async Task<RolRegistroDto> CreateAsync(RolRegistroDto dto)
        {
            var entity = new RolRegistro {
                BloqueTech = dto.BloqueTech,
                Descripcion = dto.Descripcion,
                EsActivo = dto.EsActivo,
                NombreRol = dto.NombreRol
            };
            var created = await _repo.CreateAsync(entity);
            return new RolRegistroDto {
                IdRolRegistro = created.IdRolRegistro,
                BloqueTech = created.BloqueTech,
                Descripcion = created.Descripcion,
                EsActivo = created.EsActivo,
                NombreRol = created.NombreRol
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<RolRegistroDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(r => new RolRegistroDto {
                IdRolRegistro = r.IdRolRegistro,
                BloqueTech = r.BloqueTech,
                Descripcion = r.Descripcion,
                EsActivo = r.EsActivo,
                NombreRol = r.NombreRol
            });
        }

        public async Task<RolRegistroDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new RolRegistroDto {
                IdRolRegistro = e.IdRolRegistro,
                BloqueTech = e.BloqueTech,
                Descripcion = e.Descripcion,
                EsActivo = e.EsActivo,
                NombreRol = e.NombreRol
            };
        }

        public async Task<RolRegistroDto?> UpdateAsync(int id, RolRegistroDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.BloqueTech = dto.BloqueTech;
            e.Descripcion = dto.Descripcion;
            e.EsActivo = dto.EsActivo;
            e.NombreRol = dto.NombreRol;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
