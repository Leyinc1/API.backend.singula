using Singula.Core.Core.Entities;
using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto>> GetAllAsync();
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto);
        Task<UsuarioDto?> UpdateAsync(int id, UpdateUsuarioDto dto);
        Task<bool> DeleteAsync(int id);
        Task<string?> AuthenticateAsync(AuthRequestDto request);
        
        // Métodos de perfil de usuario
        Task<UsuarioDto?> UpdateProfileAsync(int userId, UserProfileUpdateDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
        
        // Método temporal de administración - ELIMINAR DESPUÉS DE USAR
        Task<bool> ForceResetAdminPasswordAsync(string newPassword);
    }
}