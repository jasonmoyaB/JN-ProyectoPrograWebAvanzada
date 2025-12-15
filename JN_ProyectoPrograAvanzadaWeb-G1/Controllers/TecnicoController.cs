using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaWeb_G1.Services;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class TecnicoController : Controller
    {
        private readonly ILogger<TecnicoController> _logger;
        private readonly IApiInventarioService _inventarioService;
        private readonly IApiMovimientoService _movimientoService;
        private readonly IApiSolicitudService _solicitudService;
        private readonly IApiProductoService _productoService;

        public TecnicoController(
            ILogger<TecnicoController> logger,
            IApiInventarioService inventarioService,
            IApiMovimientoService movimientoService,
            IApiSolicitudService solicitudService,
            IApiProductoService productoService)
        {
            _logger = logger;
            _inventarioService = inventarioService;
            _movimientoService = movimientoService;
            _solicitudService = solicitudService;
            _productoService = productoService;
        }


        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 2)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            var bodegaId = HttpContext.Session.GetInt32("BodegaID");

            ViewBag.UsuarioNombre = HttpContext.Session.GetString("Usuario") ?? "Técnico";
            ViewBag.RolNombre = HttpContext.Session.GetString("RolNombre") ?? "Técnico";
            ViewBag.BodegaID = bodegaId;
            ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Sin bodega asignada";

            try
            {
                if (bodegaId.HasValue)
                {
                    ViewBag.TotalProductos = await _inventarioService.GetTotalProductosByBodegaAsync(bodegaId.Value);
                    ViewBag.StockBajo = await _inventarioService.GetTotalStockBajoByBodegaAsync(bodegaId.Value);
                    ViewBag.MovimientosHoy = await _movimientoService.GetCountMovimientosHoyByBodegaAsync(bodegaId.Value);
                }
                else
                {
                    ViewBag.TotalProductos = 0;
                    ViewBag.StockBajo = 0;
                    ViewBag.MovimientosHoy = 0;
                }

                if (usuarioId.HasValue)
                {
                    ViewBag.MisSolicitudes = await _solicitudService.GetCountPendientesByUsuarioAsync(usuarioId.Value);
                }
                else
                {
                    ViewBag.MisSolicitudes = 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar estadísticas del dashboard");
                ViewBag.TotalProductos = 0;
                ViewBag.StockBajo = 0;
                ViewBag.MisSolicitudes = 0;
                ViewBag.MovimientosHoy = 0;
            }
            
            return View();
        }

        // GET: Tecnico/MiInventario
        [HttpGet]
        public async Task<IActionResult> MiInventario()
        {
            try
            {
                var rolId = HttpContext.Session.GetInt32("RolID");
                if (rolId != 2)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                var bodegaId = HttpContext.Session.GetInt32("BodegaID");
                if (!bodegaId.HasValue)
                {
                    TempData["Error"] = "No tienes una bodega asignada. Contacta al administrador.";
                    return RedirectToAction(nameof(Dashboard));
                }

                var inventario = await _inventarioService.GetSaldoByBodegaAsync(bodegaId.Value);
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                return View(inventario ?? new List<SaldoInventarioDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar inventario: {Message}", ex.Message);
                TempData["Error"] = $"Error al cargar el inventario: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // GET: Tecnico/Movimientos
        [HttpGet]
        public async Task<IActionResult> Movimientos(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                var rolId = HttpContext.Session.GetInt32("RolID");
                if (rolId != 2)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                var bodegaId = HttpContext.Session.GetInt32("BodegaID");
                if (!bodegaId.HasValue)
                {
                    TempData["Error"] = "No tienes una bodega asignada. Contacta al administrador.";
                    return RedirectToAction(nameof(Dashboard));
                }

                _logger.LogInformation("Técnico: Obteniendo movimientos para bodega {BodegaId}", bodegaId.Value);
                var movimientos = await _movimientoService.GetByBodegaAsync(bodegaId.Value, fechaDesde, fechaHasta);
                _logger.LogInformation("Técnico: Se encontraron {Count} movimientos para bodega {BodegaId}", movimientos?.Count ?? 0, bodegaId.Value);
                
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                ViewBag.FechaDesde = fechaDesde;
                ViewBag.FechaHasta = fechaHasta;
                return View(movimientos ?? new List<MovimientoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar movimientos: {Message}", ex.Message);
                TempData["Error"] = $"Error al cargar los movimientos: {ex.Message}";
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                return View(new List<MovimientoDto>());
            }
        }

        // GET: Tecnico/Solicitudes
        [HttpGet]
        public async Task<IActionResult> Solicitudes()
        {
            try
            {
                var rolId = HttpContext.Session.GetInt32("RolID");
                if (rolId != 2)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
                if (!usuarioId.HasValue)
                {
                    TempData["Error"] = "Error al identificar el usuario";
                    return RedirectToAction(nameof(Dashboard));
                }

                var solicitudes = await _solicitudService.GetByUsuarioAsync(usuarioId.Value);
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                return View(solicitudes ?? new List<SolicitudDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar solicitudes: {Message}", ex.Message);
                TempData["Error"] = $"Error al cargar las solicitudes: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
            }
        }

        // GET: Tecnico/Solicitudes/Crear
        [HttpGet]
        public async Task<IActionResult> CrearSolicitud()
        {
            try
            {
                var rolId = HttpContext.Session.GetInt32("RolID");
                if (rolId != 2)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                var bodegaId = HttpContext.Session.GetInt32("BodegaID");
                if (!bodegaId.HasValue)
                {
                    TempData["Error"] = "No tienes una bodega asignada. Contacta al administrador.";
                    return RedirectToAction(nameof(Dashboard));
                }

                var productos = await _productoService.GetAllAsync(true);
                _logger.LogInformation("Productos cargados para solicitud: {Count}", productos?.Count ?? 0);
                
                if (productos == null || !productos.Any())
                {
                    _logger.LogWarning("No se encontraron productos activos para la solicitud");
                    TempData["Warning"] = "No hay productos disponibles. Contacta al administrador.";
                }

                
                var inventarioBodegaGeneral = await _inventarioService.GetSaldoByBodegaAsync(1);
                
               
                var productosConStock = productos != null ? productos.Select(p =>
                {
                    var saldo = inventarioBodegaGeneral?.FirstOrDefault(i => i.ProductoID == p.ProductoID);
                    return new
                    {
                        p.ProductoID,
                        p.SKU,
                        p.Nombre,
                        CantidadDisponible = saldo?.Cantidad ?? 0
                    };
                }).Cast<object>().ToList() : new List<object>();
                
                ViewBag.Productos = productosConStock;
                ViewBag.BodegaID = bodegaId.Value;
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                return View(new CrearSolicitudDto { BodegaID = bodegaId.Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de solicitud: {Message}", ex.Message);
                TempData["Error"] = $"Error al cargar el formulario: {ex.Message}";
                return RedirectToAction(nameof(Solicitudes));
            }
        }

        // POST: Tecnico/Solicitudes/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSolicitud(CrearSolicitudDto dto)
        {
            try
            {
                var rolId = HttpContext.Session.GetInt32("RolID");
                if (rolId != 2)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
                if (!usuarioId.HasValue)
                {
                    TempData["Error"] = "Error al identificar el usuario";
                    return RedirectToAction(nameof(Solicitudes));
                }

                if (!ModelState.IsValid)
                {
                    var productos = await _productoService.GetAllAsync(true);
                    var inventarioBodegaGeneral = await _inventarioService.GetSaldoByBodegaAsync(1);
                    
                    var productosConStock = productos != null ? productos.Select(p =>
                    {
                        var saldo = inventarioBodegaGeneral?.FirstOrDefault(i => i.ProductoID == p.ProductoID);
                        return new
                        {
                            p.ProductoID,
                            p.SKU,
                            p.Nombre,
                            CantidadDisponible = saldo?.Cantidad ?? 0
                        };
                    }).Cast<object>().ToList() : new List<object>();
                    
                    ViewBag.Productos = productosConStock;
                    ViewBag.BodegaID = dto.BodegaID;
                    ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                    return View(dto);
                }

                await _solicitudService.CreateAsync(dto, usuarioId.Value);
                TempData["Success"] = "Solicitud creada exitosamente";
                return RedirectToAction(nameof(Solicitudes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud: {Message}", ex.Message);
                TempData["Error"] = $"Error al crear la solicitud: {ex.Message}";
                try
                {
                    var productos = await _productoService.GetAllAsync(true);
                    var inventarioBodegaGeneral = await _inventarioService.GetSaldoByBodegaAsync(1);
                    
                    var productosConStock = productos != null ? productos.Select(p =>
                    {
                        var saldo = inventarioBodegaGeneral?.FirstOrDefault(i => i.ProductoID == p.ProductoID);
                        return new
                        {
                            p.ProductoID,
                            p.SKU,
                            p.Nombre,
                            CantidadDisponible = saldo?.Cantidad ?? 0
                        };
                    }).Cast<object>().ToList() : new List<object>();
                    
                    ViewBag.Productos = productosConStock;
                }
                catch
                {
                    ViewBag.Productos = new List<object>();
                }
                ViewBag.BodegaID = dto.BodegaID;
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                return View(dto);
            }
        }

        // GET: Tecnico/Movimientos/Detalle
        [HttpGet]
        public async Task<IActionResult> DetalleMovimiento(int id)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 2)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            try
            {
                var movimiento = await _movimientoService.GetByIdAsync(id);
                if (movimiento == null)
                {
                    TempData["Error"] = "Movimiento no encontrado";
                    return RedirectToAction(nameof(Movimientos));
                }

                
                var bodegaId = HttpContext.Session.GetInt32("BodegaID");
                if (bodegaId.HasValue && movimiento.BodegaID != bodegaId.Value)
                {
                    TempData["Error"] = "No tienes permiso para ver este movimiento";
                    return RedirectToAction(nameof(Movimientos));
                }

                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                return View(movimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalle de movimiento");
                TempData["Error"] = "Error al cargar el movimiento";
                return RedirectToAction(nameof(Movimientos));
            }
        }

        // GET: Tecnico/Solicitudes/Detalle
        [HttpGet]
        public async Task<IActionResult> DetalleSolicitud(int id)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 2)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            try
            {
                var solicitud = await _solicitudService.GetByIdAsync(id);
                if (solicitud == null)
                {
                    TempData["Error"] = "Solicitud no encontrada";
                    return RedirectToAction(nameof(Solicitudes));
                }

                return View(solicitud);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalle de solicitud");
                TempData["Error"] = "Error al cargar la solicitud";
                return RedirectToAction(nameof(Solicitudes));
            }
        }
    }
}

