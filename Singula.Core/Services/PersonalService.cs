using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class PersonalService : IPersonalService
    {
        private readonly IRepository<Personal> _repo;

        public PersonalService(IRepository<Personal> repo)
        {
            _repo = repo;
        }

        public async Task<PersonalDto> CreateAsync(PersonalDto dto)
        {
            var entity = new Personal {
                IdUsuario = dto.IdUsuario,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Documento = dto.Documento,
                Estado = dto.Estado
            };
            var created = await _repo.CreateAsync(entity);
            return new PersonalDto {
                IdPersonal = created.IdPersonal,
                IdUsuario = created.IdUsuario,
                Nombres = created.Nombres,
                Apellidos = created.Apellidos,
                Documento = created.Documento,
                Estado = created.Estado,
                CreadoEn = created.CreadoEn,
                ActualizadoEn = created.ActualizadoEn
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return false;
            e.Estado = "deleted";
            await _repo.UpdateAsync(e);
            return true;
        }

        public async Task<IEnumerable<PersonalDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(p => new PersonalDto {
                IdPersonal = p.IdPersonal,
                IdUsuario = p.IdUsuario,
                Nombres = p.Nombres,
                Apellidos = p.Apellidos,
                Documento = p.Documento,
                Estado = p.Estado,
                CreadoEn = p.CreadoEn,
                ActualizadoEn = p.ActualizadoEn
            });
        }

        public async Task<PersonalDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new PersonalDto {
                IdPersonal = e.IdPersonal,
                IdUsuario = e.IdUsuario,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                Documento = e.Documento,
                Estado = e.Estado,
                CreadoEn = e.CreadoEn,
                ActualizadoEn = e.ActualizadoEn
            };
        }

        public async Task<PersonalDto?> UpdateAsync(int id, PersonalDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.Nombres = dto.Nombres;
            e.Apellidos = dto.Apellidos;
            e.Documento = dto.Documento;
            e.Estado = dto.Estado;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
