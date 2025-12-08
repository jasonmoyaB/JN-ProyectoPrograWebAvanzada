using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Inventario;
using JN_ProyectoPrograAvanzadaApi_G1.Application.Services;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;
        private readonly ILogger<InventarioController> _logger;

        public InventarioController(IInventarioService inventarioService, ILogger<InventarioController> logger)
        {
            _inventarioService = inventarioService;
            _logger = logger;
        }

        [HttpGet("bodega/{bodegaId}")]
        public async Task<ActionResult<List<SaldoInventarioDto>>> GetByBodega(int bodegaId)
        {
            try
            {
                var saldos = await _inventarioService.GetSaldoByBodegaAsync(bodegaId);
                return Ok(saldos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener inventario por bodega");
                return StatusCode(500, new { message = "Error al obtener inventario" });
            }
        }

        [HttpGet("bodega/{bodegaId}/producto/{productoId}")]
        public async Task<ActionResult<SaldoInventarioDto>> GetByBodegaAndProducto(int bodegaId, int productoId)
        {
            try
            {
                var saldo = await _inventarioService.GetSaldoByBodegaAndProductoAsync(bodegaId, productoId);
                if (saldo == null)
                {
                    return NotFound(new { message = "Saldo no encontrado" });
                }
                return Ok(saldo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener saldo");
                return StatusCode(500, new { message = "Error al obtener saldo" });
            }
        }

        [HttpGet("bodega/{bodegaId}/stock-bajo")]
        public async Task<ActionResult<List<SaldoInventarioDto>>> GetStockBajo(int bodegaId)
        {
            try
            {
                var saldos = await _inventarioService.GetSaldosConAlertaStockBajoByBodegaAsync(bodegaId);
                return Ok(saldos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener stock bajo");
                return StatusCode(500, new { message = "Error al obtener stock bajo" });
            }
        }

        [HttpGet("bodega/{bodegaId}/resumen")]
        public async Task<ActionResult> GetResumen(int bodegaId)
        {
            try
            {
                var totalProductos = await _inventarioService.GetTotalProductosByBodegaAsync(bodegaId);
                var totalStockBajo = await _inventarioService.GetTotalStockBajoByBodegaAsync(bodegaId);

                return Ok(new
                {
                    TotalProductos = totalProductos,
                    TotalStockBajo = totalStockBajo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen");
                return StatusCode(500, new { message = "Error al obtener resumen" });
            }
        }
    }
}

