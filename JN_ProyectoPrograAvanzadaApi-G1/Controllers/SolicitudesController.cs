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

        [HttpGet]
        public async Task<ActionResult<List<SolicitudDto>>> GetAll()
        {
            try
            {
                var solicitudes = await _solicitudService.GetAllAsync();
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las solicitudes");
                return StatusCode(500, new { message = "Error al obtener las solicitudes" });
            }
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

        [HttpPut("{id}/aprobar")]
        public async Task<ActionResult> Aprobar(int id, [FromBody] AprobarRechazarRequest request)
        {
            try
            {
                
                var usuarioAprobadorId = request.UsuarioID > 0 ? request.UsuarioID : 1; 
                await _solicitudService.AprobarAsync(id, usuarioAprobadorId, request.Comentarios);
                return Ok(new { message = "Solicitud aprobada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aprobar solicitud {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/rechazar")]
        public async Task<ActionResult> Rechazar(int id, [FromBody] AprobarRechazarRequest request)
        {
            try
            {
                var usuarioAprobadorId = request.UsuarioID > 0 ? request.UsuarioID : 1; 
                await _solicitudService.RechazarAsync(id, usuarioAprobadorId, request.Comentarios);
                return Ok(new { message = "Solicitud rechazada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al rechazar solicitud {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/enviar")]
        public async Task<ActionResult> Enviar(int id, [FromBody] EnviarRequest request)
        {
            try
            {
                await _solicitudService.EnviarAsync(id, request.Comentarios);
                return Ok(new { message = "Solicitud enviada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar solicitud {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/entregar")]
        public async Task<ActionResult> Entregar(int id, [FromBody] EntregarRequest request)
        {
            try
            {
                await _solicitudService.EntregarAsync(id, request.BodegaOrigenID, request.BodegaDestinoID, request.UsuarioID, request.Comentarios);
                return Ok(new { message = "Solicitud entregada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al entregar solicitud {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}/establecer-cantidades")]
        public async Task<ActionResult> EstablecerCantidadesEnviadas(int id, [FromBody] EstablecerCantidadesEnviadasDto dto)
        {
            try
            {
                await _solicitudService.EstablecerCantidadesEnviadasAsync(id, dto);
                return Ok(new { message = "Cantidades establecidas exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer cantidades para solicitud {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/tiene-cantidades-enviadas")]
        public async Task<ActionResult> TieneCantidadesEnviadas(int id)
        {
            try
            {
                var tiene = await _solicitudService.TieneCantidadesEnviadasAsync(id);
                return Ok(new { tieneCantidades = tiene });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar cantidades enviadas para solicitud {Id}", id);
                return StatusCode(500, new { message = "Error al verificar cantidades" });
            }
        }
    }

    public class CrearSolicitudRequest
    {
        public CrearSolicitudDto Dto { get; set; } = new();
        public int UsuarioID { get; set; }
    }

    public class AprobarRechazarRequest
    {
        public string? Comentarios { get; set; }
        public int UsuarioID { get; set; }
    }

    public class EnviarRequest
    {
        public string? Comentarios { get; set; }
    }

    public class EntregarRequest
    {
        public int BodegaOrigenID { get; set; }
        public int BodegaDestinoID { get; set; }
        public int UsuarioID { get; set; }
        public string? Comentarios { get; set; }
    }
}

