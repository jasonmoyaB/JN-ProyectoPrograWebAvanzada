using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Usuarios;
using JN_ProyectoPrograAvanzadaApi_G1.Application.Services;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<UsuarioDto>>> GetAll([FromQuery] bool? activo = null)
        {
            try
            {
                var usuarios = await _usuarioService.GetAllAsync(activo);
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = "Error al obtener usuarios" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetById(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetByIdAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario");
                return StatusCode(500, new { message = "Error al obtener usuario" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CrearUsuarioDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuarioId = await _usuarioService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = usuarioId }, new { id = usuarioId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, new { message = "Error al crear usuario" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] EditarUsuarioDto dto)
        {
            try
            {
                _logger.LogInformation("Recibida petición para actualizar usuario {Id} con datos: Nombre={Nombre}, RolID={RolID}, BodegaID={BodegaID}, Activo={Activo}", 
                    id, dto?.Nombre, dto?.RolID, dto?.BodegaID, dto?.Activo);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("ModelState inválido para usuario {Id}. Errores: {Errors}", id, string.Join(", ", errors));
                    return BadRequest(new { message = "Datos inválidos", errors = errors });
                }

                dto.UsuarioID = id;
                var resultado = await _usuarioService.UpdateAsync(id, dto);
                
                if (!resultado)
                {
                    _logger.LogWarning("Usuario {Id} no encontrado o no se pudo actualizar", id);
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                _logger.LogInformation("Usuario {Id} actualizado exitosamente", id);
                return Ok(new { message = "Usuario actualizado correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de operación al actualizar usuario {Id}: {Message}", id, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar usuario {Id}", id);
                return StatusCode(500, new { message = "Error al actualizar usuario" });
            }
        }

        [HttpPatch("{id}/toggle-activo")]
        public async Task<ActionResult> ToggleActivo(int id)
        {
            try
            {
                var resultado = await _usuarioService.ToggleActivoAsync(id);
                if (!resultado)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del usuario");
                return StatusCode(500, new { message = "Error al cambiar estado" });
            }
        }

        [HttpPut("{id}/asignar-bodega")]
        public async Task<ActionResult> AssignarBodega(int id, [FromBody] AssignarBodegaRequest request)
        {
            try
            {
                var resultado = await _usuarioService.AssignarBodegaAsync(id, request.BodegaID);
                if (!resultado)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                return Ok(new { message = "Bodega asignada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar bodega");
                return StatusCode(500, new { message = "Error al asignar bodega" });
            }
        }
    }

    public class AssignarBodegaRequest
    {
        public int? BodegaID { get; set; }
    }
}

