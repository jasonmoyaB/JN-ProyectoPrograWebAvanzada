using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Solicitudes;
using JN_ProyectoPrograAvanzadaApi_G1.Application.Services;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudService _solicitudService;
        private readonly ILogger<SolicitudesController> _logger;

        public SolicitudesController(ISolicitudService solicitudService, ILogger<SolicitudesController> logger)
        {
            _solicitudService = solicitudService;
            _logger = logger;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<List<SolicitudDto>>> GetByUsuario(int usuarioId)
        {
            try
            {
                var solicitudes = await _solicitudService.GetByUsuarioAsync(usuarioId);
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes por usuario");
                return StatusCode(500, new { message = "Error al obtener solicitudes" });
            }
        }

        [HttpGet("bodega/{bodegaId}")]
        public async Task<ActionResult<List<SolicitudDto>>> GetByBodega(int bodegaId)
        {
            try
            {
                var solicitudes = await _solicitudService.GetByBodegaAsync(bodegaId);
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes por bodega");
                return StatusCode(500, new { message = "Error al obtener solicitudes" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudDto>> GetById(int id)
        {
            try
            {
                var solicitud = await _solicitudService.GetByIdAsync(id);
                if (solicitud == null)
                {
                    return NotFound(new { message = "Solicitud no encontrada" });
                }
                return Ok(solicitud);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitud");
                return StatusCode(500, new { message = "Error al obtener solicitud" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CrearSolicitudRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var solicitudId = await _solicitudService.CreateAsync(request.Dto, request.UsuarioID);
                return CreatedAtAction(nameof(GetById), new { id = solicitudId }, new { id = solicitudId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("usuario/{usuarioId}/count-pendientes")]
        public async Task<ActionResult> GetCountPendientes(int usuarioId)
        {
            try
            {
                var count = await _solicitudService.GetCountPendientesByUsuarioAsync(usuarioId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conteo de solicitudes pendientes");
                return StatusCode(500, new { message = "Error al obtener conteo" });
            }
        }
    }

    public class CrearSolicitudRequest
    {
        public CrearSolicitudDto Dto { get; set; } = new();
        public int UsuarioID { get; set; }
    }
}

