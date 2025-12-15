using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaWeb_G1.Services;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class TrasladosController : Controller
    {
        private readonly IApiBodegaService _bodegaService;
        private readonly IApiProductoService _productoService;
        private readonly IApiInventarioService _inventarioService;
        private readonly IApiMovimientoService _movimientoService;
        private readonly ILogger<TrasladosController> _logger;

        public TrasladosController(
            IApiBodegaService bodegaService,
            IApiProductoService productoService,
            IApiInventarioService inventarioService,
            IApiMovimientoService movimientoService,
            ILogger<TrasladosController> logger)
        {
            _bodegaService = bodegaService;
            _productoService = productoService;
            _inventarioService = inventarioService;
            _movimientoService = movimientoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                
                var bodegas = await _bodegaService.GetAllAsync(true);
                var bodegaGeneral = bodegas.FirstOrDefault(b => b.BodegaID == 1);
                var bodegasDestino = bodegas.Where(b => b.BodegaID != 1).ToList();

                ViewBag.BodegaOrigen = bodegaGeneral;
                ViewBag.BodegasDestino = bodegasDestino;

                
                var productos = await _productoService.GetAllAsync(true);
                var inventarioBodegaGeneral = await _inventarioService.GetSaldoByBodegaAsync(1);

               
                var productosConStock = productos
                    .Where(p => inventarioBodegaGeneral.Any(i => i.ProductoID == p.ProductoID && i.Cantidad > 0))
                    .Select(p =>
                    {
                        var saldo = inventarioBodegaGeneral.FirstOrDefault(i => i.ProductoID == p.ProductoID);
                        return new
                        {
                            p.ProductoID,
                            p.Nombre,
                            p.SKU,
                            StockDisponible = saldo?.Cantidad ?? 0
                        };
                    })
                    .ToList();

                ViewBag.Productos = productosConStock;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para traslado");
                TempData["Error"] = "Error al cargar los datos necesarios para el traslado";
                return RedirectToAction("Index", "Movimientos");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrasladoViewModel model)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            if (!ModelState.IsValid || model.Detalles == null || !model.Detalles.Any())
            {
                TempData["Error"] = "Debe incluir al menos un producto para el traslado";
                return RedirectToAction(nameof(Create));
            }

            try
            {
                
                if (model.BodegaOrigenID != 1)
                {
                    TempData["Error"] = "La bodega de origen debe ser Bodega General";
                    return RedirectToAction(nameof(Create));
                }

                
                if (model.BodegaOrigenID == model.BodegaDestinoID)
                {
                    TempData["Error"] = "La bodega de origen y destino deben ser diferentes";
                    return RedirectToAction(nameof(Create));
                }

                
                var inventarioBodegaGeneral = await _inventarioService.GetSaldoByBodegaAsync(1);
                foreach (var detalle in model.Detalles)
                {
                    var saldo = inventarioBodegaGeneral.FirstOrDefault(i => i.ProductoID == detalle.ProductoID);
                    if (saldo == null || saldo.Cantidad < detalle.Cantidad)
                    {
                        TempData["Error"] = $"No hay suficiente stock disponible para el producto seleccionado";
                        return RedirectToAction(nameof(Create));
                    }
                }

               
                if (string.IsNullOrWhiteSpace(model.Referencia))
                {
                    model.Referencia = $"TRAS-{DateTime.UtcNow:yyyyMMddHHmmss}";
                }

                
                var dto = new MovimientoTrasladoDto
                {
                    BodegaOrigenID = model.BodegaOrigenID,
                    BodegaDestinoID = model.BodegaDestinoID,
                    Referencia = model.Referencia,
                    Observaciones = model.Observaciones,
                    Detalles = model.Detalles.Select(d => new MovimientoTrasladoDetalleDto
                    {
                        ProductoID = d.ProductoID,
                        Cantidad = d.Cantidad,
                        UnidadID = 0 
                    }).ToList()
                };

                // Llamar al API para crear el traslado
                var usuarioId = HttpContext.Session.GetInt32("UsuarioID") ?? 0;
                if (usuarioId == 0)
                {
                    TempData["Error"] = "No se pudo identificar al usuario";
                    return RedirectToAction(nameof(Create));
                }

                var movimientoId = await _movimientoService.CreateTrasladoAsync(dto, usuarioId);
                
                TempData["Success"] = $"Traslado creado exitosamente. Referencia: {model.Referencia}";
                return RedirectToAction("Index", "Movimientos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear traslado: {Message}", ex.Message);
                var errorMessage = "Error al procesar el traslado";
                
                
                if (ex.InnerException != null)
                {
                    errorMessage += $": {ex.InnerException.Message}";
                }
                else if (!string.IsNullOrEmpty(ex.Message))
                {
                    errorMessage += $": {ex.Message}";
                }
                
                TempData["Error"] = errorMessage;
                
                
                try
                {
                    var bodegas = await _bodegaService.GetAllAsync(true);
                    var bodegaGeneral = bodegas.FirstOrDefault(b => b.BodegaID == 1);
                    var bodegasDestino = bodegas.Where(b => b.BodegaID != 1).ToList();
                    ViewBag.BodegaOrigen = bodegaGeneral;
                    ViewBag.BodegasDestino = bodegasDestino;
                    
                    var productos = await _productoService.GetAllAsync(true);
                    var inventarioBodegaGeneral = await _inventarioService.GetSaldoByBodegaAsync(1);
                    var productosConStock = productos
                        .Where(p => inventarioBodegaGeneral.Any(i => i.ProductoID == p.ProductoID && i.Cantidad > 0))
                        .Select(p =>
                        {
                            var saldo = inventarioBodegaGeneral.FirstOrDefault(i => i.ProductoID == p.ProductoID);
                            return new
                            {
                                p.ProductoID,
                                p.Nombre,
                                p.SKU,
                                StockDisponible = saldo?.Cantidad ?? 0
                            };
                        })
                        .ToList();
                    ViewBag.Productos = productosConStock;
                }
                catch
                {
                    
                }
                
                return View(model);
            }
        }
    }

    public class TrasladoViewModel
    {
        public int BodegaOrigenID { get; set; } = 1;
        public int BodegaDestinoID { get; set; }
        public string? Referencia { get; set; }
        public string? Observaciones { get; set; }
        public List<TrasladoDetalleViewModel> Detalles { get; set; } = new List<TrasladoDetalleViewModel>();
    }

    public class TrasladoDetalleViewModel
    {
        public int ProductoID { get; set; }
        public decimal Cantidad { get; set; }
    }
}

