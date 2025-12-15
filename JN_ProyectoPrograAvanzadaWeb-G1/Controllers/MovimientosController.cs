using JN_ProyectoPrograAvanzadaWeb_G1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class MovimientosController : Controller
    {
        private readonly IApiMovimientoService _movimientoService;
        private readonly IApiBodegaService _bodegaService;
        private readonly ILogger<MovimientosController> _logger;

        public MovimientosController(
            IApiMovimientoService movimientoService,
            IApiBodegaService bodegaService,
            ILogger<MovimientosController> logger)
        {
            _movimientoService = movimientoService;
            _bodegaService = bodegaService;
            _logger = logger;
        }

        // GET: Movimientos
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? bodegaId = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                List<MovimientoDto> movimientos = new List<MovimientoDto>();

                if (bodegaId.HasValue)
                {
                    _logger.LogInformation("Obteniendo movimientos para bodega {BodegaId}", bodegaId.Value);
                    movimientos = await _movimientoService.GetByBodegaAsync(bodegaId.Value, fechaDesde, fechaHasta);
                    _logger.LogInformation("Se encontraron {Count} movimientos para bodega {BodegaId}", movimientos?.Count ?? 0, bodegaId.Value);
                }
                else
                {
                    // Obtener todas las bodegas y sus movimientos
                    var bodegas = await _bodegaService.GetAllAsync(true);
                    _logger.LogInformation("Obteniendo movimientos para todas las bodegas. Total bodegas: {Count}", bodegas?.Count ?? 0);
                    var todosMovimientos = new List<MovimientoDto>();

                    foreach (var bodega in bodegas ?? new List<BodegaDto>())
                    {
                        try
                        {
                            var movs = await _movimientoService.GetByBodegaAsync(bodega.BodegaID, fechaDesde, fechaHasta);
                            if (movs != null && movs.Any())
                            {
                                _logger.LogInformation("Bodega {BodegaId} ({Nombre}): {Count} movimientos", bodega.BodegaID, bodega.Nombre, movs.Count);
                                todosMovimientos.AddRange(movs);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error al obtener movimientos para bodega {BodegaId}", bodega.BodegaID);
                        }
                    }

                    movimientos = todosMovimientos.OrderByDescending(m => m.FechaMovimientoUTC).ToList();
                    _logger.LogInformation("Total de movimientos encontrados: {Count}", movimientos.Count);
                }

                var bodegasList = await _bodegaService.GetAllAsync(true);
                ViewBag.Bodegas = bodegasList ?? new List<BodegaDto>();
                ViewBag.BodegaId = bodegaId;
                ViewBag.FechaDesde = fechaDesde;
                ViewBag.FechaHasta = fechaHasta;

                return View(movimientos ?? new List<MovimientoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar movimientos: {Message}", ex.Message);
                TempData["Error"] = $"Error al cargar los movimientos: {ex.Message}";
                var bodegasList = await _bodegaService.GetAllAsync(true);
                ViewBag.Bodegas = bodegasList ?? new List<BodegaDto>();
                return View(new List<MovimientoDto>());
            }
        }

        // GET: Movimientos/Detalles
        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                var movimiento = await _movimientoService.GetByIdAsync(id);
                if (movimiento == null)
                {
                    TempData["Error"] = "Movimiento no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                return View(movimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalle de movimiento");
                TempData["Error"] = "Error al cargar el movimiento";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
