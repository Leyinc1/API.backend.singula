using AutoMapper;
using Singula.Core.Core.Entities;
using Singula.Core.Services.Dto;

namespace Singula.Core.Mapping
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioDto>();
            CreateMap<CreateUsuarioDto, Usuario>();
        }
    }
}