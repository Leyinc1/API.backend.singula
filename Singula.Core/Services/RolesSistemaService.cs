using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class RolesSistemaService : IRolesSistemaService
    {
        private readonly IRepository<RolesSistema> _repo;

        public RolesSistemaService(IRepository<RolesSistema> repo)
        {
            _repo = repo;
        }

        public async Task<RolesSistemaDto> CreateAsync(RolesSistemaDto dto)
        {
            var entity = new RolesSistema {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                EsActivo = dto.EsActivo,
                Nombre = dto.Nombre
            };
            var created = await _repo.CreateAsync(entity);
            return new RolesSistemaDto {
                IdRolSistema = created.IdRolSistema,
                Codigo = created.Codigo,
                Descripcion = created.Descripcion,
                EsActivo = created.EsActivo,
                Nombre = created.Nombre
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<RolesSistemaDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(r => new RolesSistemaDto {
                IdRolSistema = r.IdRolSistema,
                Codigo = r.Codigo,
                Descripcion = r.Descripcion,
                EsActivo = r.EsActivo,
                Nombre = r.Nombre
            });
        }

        public async Task<RolesSistemaDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new RolesSistemaDto {
                IdRolSistema = e.IdRolSistema,
                Codigo = e.Codigo,
                Descripcion = e.Descripcion,
                EsActivo = e.EsActivo,
                Nombre = e.Nombre
            };
        }

        public async Task<RolesSistemaDto?> UpdateAsync(int id, RolesSistemaDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.Codigo = dto.Codigo;
            e.Descripcion = dto.Descripcion;
            e.EsActivo = dto.EsActivo;
            e.Nombre = dto.Nombre;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
