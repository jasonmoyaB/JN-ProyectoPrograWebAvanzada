using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Bodegas;
using JN_ProyectoPrograAvanzadaApi_G1.Application.Services;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
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
                return StatusCode(500, new { message = "Error al obtener bodegas" });
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
                _logger.LogError(ex, "Error al obtener bodega");
                return StatusCode(500, new { message = "Error al obtener bodega" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CrearBodegaDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bodegaId = await _bodegaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = bodegaId }, new { id = bodegaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear bodega");
                return StatusCode(500, new { message = "Error al crear bodega" });
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

                var resultado = await _bodegaService.UpdateAsync(id, dto);
                if (!resultado)
                {
                    return NotFound(new { message = "Bodega no encontrada" });
                }

                return Ok(new { message = "Bodega actualizada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar bodega");
                return StatusCode(500, new { message = "Error al actualizar bodega" });
            }
        }

        [HttpPatch("{id}/toggle-activo")]
        public async Task<ActionResult> ToggleActivo(int id)
        {
            try
            {
                var resultado = await _bodegaService.ToggleActivoAsync(id);
                if (!resultado)
                {
                    return NotFound(new { message = "Bodega no encontrada" });
                }
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de la bodega");
                return StatusCode(500, new { message = "Error al cambiar estado" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _bodegaService.DeleteAsync(id);
                if (!resultado)
                {
                    return NotFound(new { message = "Bodega no encontrada" });
                }
                return Ok(new { message = "Bodega eliminada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar bodega");
                return StatusCode(500, new { message = "Error al eliminar bodega" });
            }
        }
    }
}

