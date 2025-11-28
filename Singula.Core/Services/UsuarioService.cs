using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Singula.Core.Core.Entities;
using Singula.Core.Repositories;
using Singula.Core.Services.Dto;

namespace Singula.Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IRepository<RolesSistema> _rolesRepo;
        private readonly IRepository<EstadoUsuarioCatalogo> _estadosRepo;
        private readonly IConfiguration _config;

        public UsuarioService(IUsuarioRepository repo, IRepository<RolesSistema> rolesRepo, IRepository<EstadoUsuarioCatalogo> estadosRepo, IConfiguration config)
        {
            _repo = repo;
            _rolesRepo = rolesRepo;
            _estadosRepo = estadosRepo;
            _config = config;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            var list = new List<UsuarioDto>();
            foreach (var u in users)
            {
                list.Add(new UsuarioDto
                {
                    IdUsuario = u.IdUsuario,
                    Username = u.Username,
                    Correo = u.Correo,
                    IdRolSistema = u.IdRolSistema,
                    IdEstadoUsuario = u.IdEstadoUsuario
                });
            }
            return list;
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;
            return new UsuarioDto
            {
                IdUsuario = user.IdUsuario,
                Username = user.Username,
                Correo = user.Correo,
                IdRolSistema = user.IdRolSistema,
                IdEstadoUsuario = user.IdEstadoUsuario
            };
        }

        public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
        {
            var existing = await _repo.GetByUsernameAsync(dto.Username);
            if (existing != null) throw new Exception("Username already exists");

            // Validate role exists
            var role = await _rolesRepo.GetByIdAsync(dto.IdRolSistema);
            if (role == null) throw new ArgumentException("IdRolSistema inv�lido");

            // Validate estado exists
            var estado = await _estadosRepo.GetByIdAsync(dto.IdEstadoUsuario);
            if (estado == null) throw new ArgumentException("IdEstadoUsuario inv�lido");

            var user = new Usuario
            {
                Username = dto.Username,
                Correo = dto.Correo,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IdRolSistema = dto.IdRolSistema,
                IdEstadoUsuario = dto.IdEstadoUsuario
            };
            var created = await _repo.CreateAsync(user);
            return new UsuarioDto
            {
                IdUsuario = created.IdUsuario,
                Username = created.Username,
                Correo = created.Correo,
                IdRolSistema = created.IdRolSistema,
                IdEstadoUsuario = created.IdEstadoUsuario
            };
        }

        public async Task<UsuarioDto?> UpdateAsync(int id, UpdateUsuarioDto dto)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return null;
            user.Correo = dto.Correo ?? user.Correo;
            user.Username = dto.Username ?? user.Username;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.ActualizadoEn = DateTime.UtcNow;
            var updated = await _repo.UpdateAsync(user);
            if (updated == null) return null;
            return new UsuarioDto
            {
                IdUsuario = updated.IdUsuario,
                Username = updated.Username,
                Correo = updated.Correo,
                IdRolSistema = updated.IdRolSistema,
                IdEstadoUsuario = updated.IdEstadoUsuario
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<string?> AuthenticateAsync(AuthRequestDto request)
        {
            var user = await _repo.GetByCorreoAsync(request.Correo);
            if (user == null) return null;
            if (string.IsNullOrEmpty(user.PasswordHash)) return null;
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return null;

            var key = _config["Jwt:Key"] ?? "super_secret_key_123!";
            var issuer = _config["Jwt:Issuer"] ?? "singula";
            var audience = _config["Jwt:Audience"] ?? "singula_users";
            var expiresMinutesString = _config["Jwt:ExpiresMinutes"];
            var expiresMinutes = 60;
            if (!string.IsNullOrEmpty(expiresMinutesString) && int.TryParse(expiresMinutesString, out var m)) expiresMinutes = m;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(key);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim("UserId", user.IdUsuario.ToString()), // Claim personalizado para facilitar acceso desde frontend
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Correo)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<UsuarioDto?> UpdateProfileAsync(int userId, UserProfileUpdateDto dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null) return null;

            // Actualizar solo los campos permitidos
            if (!string.IsNullOrWhiteSpace(dto.Username))
            {
                // Verificar que el username no esté en uso por otro usuario
                var existingUser = await _repo.GetByUsernameAsync(dto.Username);
                if (existingUser != null && existingUser.IdUsuario != userId)
                {
                    throw new Exception("El nombre de usuario ya está en uso");
                }
                user.Username = dto.Username;
            }

            // Nota: Los campos Biografia y Telefono no existen en la entidad Usuario actual
            // Si necesitas estos campos, deberás agregarlos a la entidad Usuario
            // Por ahora, solo actualizamos Username

            user.ActualizadoEn = DateTime.UtcNow;
            var updated = await _repo.UpdateAsync(user);
            
            if (updated == null) return null;
            
            return new UsuarioDto
            {
                IdUsuario = updated.IdUsuario,
                Username = updated.Username,
                Correo = updated.Correo,
                IdRolSistema = updated.IdRolSistema,
                IdEstadoUsuario = updated.IdEstadoUsuario
            };
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _repo.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            // VALIDACIÓN CRÍTICA: Verificar que la contraseña actual sea correcta
            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new Exception("El usuario no tiene contraseña configurada");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.ContrasenaActual, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta");
            }

            // Hashear la nueva contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaContrasena);
            user.ActualizadoEn = DateTime.UtcNow;

            // Guardar cambios
            await _repo.UpdateAsync(user);
        }

        /// <summary>
        /// MÉTODO TEMPORAL DE ADMINISTRACIÓN - ELIMINAR DESPUÉS DE USAR
        /// Fuerza el reseteo de contraseña del usuario administrador
        /// </summary>
        public async Task<bool> ForceResetAdminPasswordAsync(string newPassword)
        {
            // Paso 1: Buscar usuario administrador por correo
            var adminUser = await _repo.GetByCorreoAsync("admi@simula.com");

            // Paso 2: Verificar existencia
            if (adminUser == null)
            {
                return false;
            }

            // Paso 3: Hashear la nueva contraseña usando BCrypt
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Paso 4: Actualizar el campo password_hash
            adminUser.PasswordHash = hashedPassword;
            adminUser.ActualizadoEn = DateTime.UtcNow;

            // Paso 5: Guardar cambios
            var updated = await _repo.UpdateAsync(adminUser);

            // Retornar true si la operación fue exitosa
            return updated != null;
        }
    }
}
