using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly IRepository<Solicitud> _repo;

        public SolicitudService(IRepository<Solicitud> repo)
        {
            _repo = repo;
        }

        public async Task<SolicitudDto> CreateAsync(SolicitudDto dto)
        {
            var entity = new Solicitud {
                IdPersonal = dto.IdPersonal,
                IdRolRegistro = dto.IdRolRegistro,
                IdSla = dto.IdSla,
                IdArea = dto.IdArea,
                IdEstadoSolicitud = dto.IdEstadoSolicitud,
                FechaSolicitud = dto.FechaSolicitud,
                FechaIngreso = dto.FechaIngreso,
                NumDiasSla = dto.NumDiasSla,
                ResumenSla = dto.ResumenSla,
                OrigenDato = dto.OrigenDato,
                CreadoPor = dto.CreadoPor
            };
            var created = await _repo.CreateAsync(entity);
            return new SolicitudDto {
                IdSolicitud = created.IdSolicitud,
                IdPersonal = created.IdPersonal,
                IdRolRegistro = created.IdRolRegistro,
                IdSla = created.IdSla,
                IdArea = created.IdArea,
                IdEstadoSolicitud = created.IdEstadoSolicitud,
                FechaSolicitud = created.FechaSolicitud,
                FechaIngreso = created.FechaIngreso,
                NumDiasSla = created.NumDiasSla,
                ResumenSla = created.ResumenSla,
                OrigenDato = created.OrigenDato,
                CreadoPor = created.CreadoPor,
                CreadoEn = created.CreadoEn,
                ActualizadoEn = created.ActualizadoEn,
                ActualizadoPor = created.ActualizadoPor
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<SolicitudDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(s => new SolicitudDto {
                IdSolicitud = s.IdSolicitud,
                IdPersonal = s.IdPersonal,
                IdRolRegistro = s.IdRolRegistro,
                IdSla = s.IdSla,
                IdArea = s.IdArea,
                IdEstadoSolicitud = s.IdEstadoSolicitud,
                FechaSolicitud = s.FechaSolicitud,
                FechaIngreso = s.FechaIngreso,
                NumDiasSla = s.NumDiasSla,
                ResumenSla = s.ResumenSla,
                OrigenDato = s.OrigenDato,
                CreadoPor = s.CreadoPor,
                CreadoEn = s.CreadoEn,
                ActualizadoEn = s.ActualizadoEn,
                ActualizadoPor = s.ActualizadoPor
            });
        }

        public async Task<SolicitudDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new SolicitudDto {
                IdSolicitud = e.IdSolicitud,
                IdPersonal = e.IdPersonal,
                IdRolRegistro = e.IdRolRegistro,
                IdSla = e.IdSla,
                IdArea = e.IdArea,
                IdEstadoSolicitud = e.IdEstadoSolicitud,
                FechaSolicitud = e.FechaSolicitud,
                FechaIngreso = e.FechaIngreso,
                NumDiasSla = e.NumDiasSla,
                ResumenSla = e.ResumenSla,
                OrigenDato = e.OrigenDato,
                CreadoPor = e.CreadoPor,
                CreadoEn = e.CreadoEn,
                ActualizadoEn = e.ActualizadoEn,
                ActualizadoPor = e.ActualizadoPor
            };
        }

        public async Task<SolicitudDto?> UpdateAsync(int id, SolicitudDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.IdPersonal = dto.IdPersonal;
            e.IdRolRegistro = dto.IdRolRegistro;
            e.IdSla = dto.IdSla;
            e.IdArea = dto.IdArea;
            e.IdEstadoSolicitud = dto.IdEstadoSolicitud;
            e.FechaSolicitud = dto.FechaSolicitud;
            e.FechaIngreso = dto.FechaIngreso;
            e.NumDiasSla = dto.NumDiasSla;
            e.ResumenSla = dto.ResumenSla;
            e.OrigenDato = dto.OrigenDato;
            e.ActualizadoPor = dto.ActualizadoPor;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
