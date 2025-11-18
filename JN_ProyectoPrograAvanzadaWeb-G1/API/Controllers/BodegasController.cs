using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Bodegas;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BodegasController : ControllerBase
    {
        private readonly IBodegaService _bodegaService;
        private readonly ILogger<BodegasController> _logger;

        public BodegasController(IBodegaService bodegaService, ILogger<BodegasController> logger)
        {
            _bodegaService = bodegaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<BodegaDto>>> GetAll([FromQuery] bool? activo = null)
        {
            try
            {
                var bodegas = await _bodegaService.GetAllAsync(activo);
                return Ok(bodegas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodegas");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BodegaDto>> GetById(int id)
        {
            try
            {
                var bodega = await _bodegaService.GetByIdAsync(id);
                
                if (bodega == null)
                {
                    return NotFound(new { message = "Bodega no encontrada" });
                }

                return Ok(bodega);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodega: {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<BodegaDto>> Create([FromBody] CrearBodegaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bodegaId = await _bodegaService.CreateAsync(dto);
                var bodega = await _bodegaService.GetByIdAsync(bodegaId);

                return CreatedAtAction(nameof(GetById), new { id = bodegaId }, bodega);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear bodega");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CrearBodegaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existe = await _bodegaService.ExistsAsync(id);
                if (!existe)
                {
                    return NotFound(new { message = "Bodega no encontrada" });
                }

                var actualizado = await _bodegaService.UpdateAsync(id, dto);
                
                if (!actualizado)
                {
                    return StatusCode(500, new { message = "Error al actualizar la bodega" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar bodega: {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPatch("{id}/toggle-activo")]
        public async Task<ActionResult> ToggleActivo(int id)
        {
            try
            {
                var existe = await _bodegaService.ExistsAsync(id);
                if (!existe)
                {
                    return NotFound(new { message = "Bodega no encontrada" });
                }

                var actualizado = await _bodegaService.ToggleActivoAsync(id);
                
                if (!actualizado)
                {
                    return StatusCode(500, new { message = "Error al cambiar el estado de la bodega" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de bodega: {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

