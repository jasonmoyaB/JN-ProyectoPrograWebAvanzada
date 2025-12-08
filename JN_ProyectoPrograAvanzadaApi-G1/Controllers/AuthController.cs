using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Auth;
using JN_ProyectoPrograAvanzadaApi_G1.Application.Services;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
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
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autenticar usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("cambiar-contrasena")]
        public async Task<ActionResult> CambiarContrasena([FromBody] CambiarContrasenaRequest request)
        {
            try
            {
                var resultado = await _authService.CambiarContrasenaAsync(
                    request.UsuarioID, 
                    request.ContrasenaActual, 
                    request.NuevaContrasena);

                if (!resultado)
                {
                    return BadRequest(new { message = "No se pudo cambiar la contraseña" });
                }

                return Ok(new { message = "Contraseña actualizada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("recuperar-clave")]
        public async Task<ActionResult> RecuperarClave([FromBody] RecuperarClaveRequest request)
        {
            try
            {
                var nuevaClave = await _authService.GenerarContrasenaTemporalAsync(request.Email);
                return Ok(new { message = "Contraseña temporal generada", claveTemporal = nuevaClave });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar clave");
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CambiarContrasenaRequest
    {
        public int UsuarioID { get; set; }
        public string ContrasenaActual { get; set; } = string.Empty;
        public string NuevaContrasena { get; set; } = string.Empty;
    }

    public class RecuperarClaveRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}

