using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Singula.Core.Services;
using Singula.Core.Services.Dto;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUsuarioDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.IdUsuario }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUsuarioDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthRequestDto req)
        {
            var token = await _service.AuthenticateAsync(req);
            if (token == null) return Unauthorized();
            return Ok(new { token });
        }

        /// <summary>
        /// Actualizar el perfil del usuario autenticado
        /// </summary>
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateDto dto)
        {
            try
            {
                // Obtener el ID del usuario desde el token JWT
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Token inválido o usuario no identificado" });
                }

                var updatedProfile = await _service.UpdateProfileAsync(userId, dto);
                if (updatedProfile == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cambiar la contraseña del usuario autenticado
        /// </summary>
        [Authorize]
        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                // Obtener el ID del usuario desde el token JWT
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Token inválido o usuario no identificado" });
                }

                await _service.ChangePasswordAsync(userId, dto);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// ?? ENDPOINT TEMPORAL DE ADMINISTRACIÓN ??
        /// Resetea la contraseña del usuario administrador (admi@simula.com)
        /// ?? ELIMINAR ESTE ENDPOINT DESPUÉS DE USARLO ??
        /// </summary>
        [AllowAnonymous]
        [HttpPost("admin/admin-unlock-temp")]
        public async Task<IActionResult> AdminUnlockTemp()
        {
            const string NewPass = "SingulaAdmin2025!";

            var success = await _service.ForceResetAdminPasswordAsync(NewPass);
            
            if (success)
            {
                return Ok(new 
                { 
                    message = $"? Contraseña de admi@simula.com reseteada exitosamente.",
                    correo = "admi@simula.com",
                    nuevaContrasena = NewPass,
                    warning = "?? IMPORTANTE: Utiliza esta contraseña para iniciar sesión y luego ELIMINA ESTE ENDPOINT por seguridad."
                });
            }
            
            return NotFound(new { message = "? Usuario admi@simula.com no encontrado en la base de datos." });
        }
    }
}