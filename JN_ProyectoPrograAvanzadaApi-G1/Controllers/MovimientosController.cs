using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Movimientos;
using JN_ProyectoPrograAvanzadaApi_G1.Application.Services;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosController : ControllerBase
    {
        private readonly IMovimientoService _movimientoService;
        private readonly ILogger<MovimientosController> _logger;

        public MovimientosController(IMovimientoService movimientoService, ILogger<MovimientosController> logger)
        {
            _movimientoService = movimientoService;
            _logger = logger;
        }

        [HttpGet("bodega/{bodegaId}")]
        public async Task<ActionResult<List<MovimientoDto>>> GetByBodega(
            int bodegaId, 
            [FromQuery] DateTime? fechaDesde = null, 
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var movimientos = await _movimientoService.GetByBodegaAsync(bodegaId, fechaDesde, fechaHasta);
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener movimientos por bodega");
                return StatusCode(500, new { message = "Error al obtener movimientos" });
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<List<MovimientoDto>>> GetByUsuario(
            int usuarioId, 
            [FromQuery] DateTime? fechaDesde = null, 
            [FromQuery] DateTime? fechaHasta = null)
        {
            try
            {
                var movimientos = await _movimientoService.GetByUsuarioAsync(usuarioId, fechaDesde, fechaHasta);
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener movimientos por usuario");
                return StatusCode(500, new { message = "Error al obtener movimientos" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoDto>> GetById(int id)
        {
            try
            {
                var movimiento = await _movimientoService.GetByIdAsync(id);
                if (movimiento == null)
                {
                    return NotFound(new { message = "Movimiento no encontrado" });
                }
                return Ok(movimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener movimiento");
                return StatusCode(500, new { message = "Error al obtener movimiento" });
            }
        }

        [HttpGet("bodega/{bodegaId}/count-hoy")]
        public async Task<ActionResult> GetCountHoyByBodega(int bodegaId)
        {
            try
            {
                var count = await _movimientoService.GetCountMovimientosHoyByBodegaAsync(bodegaId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conteo de movimientos");
                return StatusCode(500, new { message = "Error al obtener conteo" });
            }
        }

        [HttpGet("usuario/{usuarioId}/count-hoy")]
        public async Task<ActionResult> GetCountHoyByUsuario(int usuarioId)
        {
            try
            {
                var count = await _movimientoService.GetCountMovimientosHoyByUsuarioAsync(usuarioId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conteo de movimientos");
                return StatusCode(500, new { message = "Error al obtener conteo" });
            }
        }

        [HttpPost("traslado")]
        public async Task<ActionResult> CreateTraslado([FromBody] CrearTrasladoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var dto = request.Dto;
                var usuarioId = request.UsuarioID;

                if (dto.BodegaOrigenID == dto.BodegaDestinoID)
                {
                    return BadRequest(new { message = "La bodega de origen y destino deben ser diferentes" });
                }

                if (dto.Detalles == null || !dto.Detalles.Any())
                {
                    return BadRequest(new { message = "Debe incluir al menos un producto para el traslado" });
                }

                var movimientoId = await _movimientoService.CreateTrasladoAsync(dto, usuarioId);
                return Ok(new { message = "Traslado creado exitosamente", movimientoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear traslado");
                return StatusCode(500, new { message = "Error al crear el traslado: " + ex.Message });
            }
        }
    }

    public class CrearTrasladoRequest
    {
        public MovimientoTrasladoDto Dto { get; set; } = new MovimientoTrasladoDto();
        public int UsuarioID { get; set; }
    }
}

