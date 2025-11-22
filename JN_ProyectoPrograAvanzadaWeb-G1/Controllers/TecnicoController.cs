using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.Services;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class TecnicoController : Controller
    {
        private readonly ILogger<TecnicoController> _logger;
        private readonly IInventarioService _inventarioService;
        private readonly IMovimientoService _movimientoService;
        private readonly ISolicitudService _solicitudService;
        private readonly IProductoService _productoService;

        public TecnicoController(
            ILogger<TecnicoController> logger,
            IInventarioService inventarioService,
            IMovimientoService movimientoService,
            ISolicitudService solicitudService,
            IProductoService productoService)
        {
            _logger = logger;
            _inventarioService = inventarioService;
            _movimientoService = movimientoService;
            _solicitudService = solicitudService;
            _productoService = productoService;
        }

        /// Dashboard principal para Técnicos
        /// Solo accesible para usuarios con RolID = 2 (Técnico)
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
                return View(inventario ?? new List<Application.DTOs.Inventario.SaldoInventarioDto>());
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

                var movimientos = await _movimientoService.GetByBodegaAsync(bodegaId.Value, fechaDesde, fechaHasta);
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                ViewBag.FechaDesde = fechaDesde;
                ViewBag.FechaHasta = fechaHasta;
                return View(movimientos ?? new List<Application.DTOs.Movimientos.MovimientoDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar movimientos: {Message}", ex.Message);
                TempData["Error"] = $"Error al cargar los movimientos: {ex.Message}";
                return RedirectToAction(nameof(Dashboard));
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
                return View(solicitudes ?? new List<Application.DTOs.Solicitudes.SolicitudDto>());
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
                ViewBag.Productos = productos ?? new List<Application.DTOs.Productos.ProductoDto>();
                ViewBag.BodegaID = bodegaId.Value;
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                return View(new Application.DTOs.Solicitudes.CrearSolicitudDto { BodegaID = bodegaId.Value });
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
        public async Task<IActionResult> CrearSolicitud(Application.DTOs.Solicitudes.CrearSolicitudDto dto)
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
                    ViewBag.Productos = productos ?? new List<Application.DTOs.Productos.ProductoDto>();
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
                    ViewBag.Productos = productos ?? new List<Application.DTOs.Productos.ProductoDto>();
                }
                catch
                {
                    ViewBag.Productos = new List<Application.DTOs.Productos.ProductoDto>();
                }
                ViewBag.BodegaID = dto.BodegaID;
                ViewBag.BodegaNombre = HttpContext.Session.GetString("BodegaNombre") ?? "Mi Bodega";
                return View(dto);
            }
        }

        // GET: Tecnico/Solicitudes/Detalle/5
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

