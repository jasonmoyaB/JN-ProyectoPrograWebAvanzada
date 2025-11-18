using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Auth;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(request);

                if (response == null)
                {
                    return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo" });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autenticar usuario: {Email}", request.CorreoElectronico);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<LoginResponseDto>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _authService.GetUsuarioByIdAsync(id);
                
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                var response = new LoginResponseDto
                {
                    UsuarioID = usuario.UsuarioID,
                    Nombre = usuario.Nombre,
                    CorreoElectronico = usuario.CorreoElectronico,
                    RolID = usuario.RolID,
                    RolNombre = usuario.Rol?.NombreRol ?? string.Empty,
                    BodegaID = usuario.BodegaID,
                    BodegaNombre = usuario.Bodega?.Nombre,
                    Activo = usuario.Activo
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario: {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

