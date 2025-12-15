using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaWeb_G1.Services;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IApiBodegaService _bodegaService;
        private readonly IApiProductoService _productoService;
        private readonly IApiSolicitudService _solicitudService;
        private readonly IApiUsuarioService _usuarioService;
        private readonly IApiInventarioService _inventarioService;
        private readonly IApiMovimientoService _movimientoService;

        public AdminController(
            ILogger<AdminController> logger,
            IApiBodegaService bodegaService,
            IApiProductoService productoService,
            IApiSolicitudService solicitudService,
            IApiUsuarioService usuarioService,
            IApiInventarioService inventarioService,
            IApiMovimientoService movimientoService)
        {
            _logger = logger;
            _bodegaService = bodegaService;
            _productoService = productoService;
            _solicitudService = solicitudService;
            _usuarioService = usuarioService;
            _inventarioService = inventarioService;
            _movimientoService = movimientoService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            ViewBag.UsuarioNombre = HttpContext.Session.GetString("Usuario") ?? "Administrador";
            ViewBag.RolNombre = HttpContext.Session.GetString("RolNombre") ?? "Administrador";

            try
            {
                // Carga
                var bodegas = await _bodegaService.GetAllAsync(true);
                var productos = await _productoService.GetAllAsync(true);
                var solicitudes = await _solicitudService.GetAllAsync();
                var usuarios = await _usuarioService.GetAllAsync(true);

                // Contar solicitudes pendientes
                var solicitudesPendientes = solicitudes?.Count(s => s.EstadoCodigo == "Pendiente") ?? 0;

                ViewBag.TotalBodegas = bodegas?.Count ?? 0;
                ViewBag.TotalProductos = productos?.Count ?? 0;
                ViewBag.SolicitudesPendientes = solicitudesPendientes;
                ViewBag.UsuariosActivos = usuarios?.Count(u => u.Activo) ?? 0;

                // Cargar alertas de stock bajo de todas las bodegas
                var todasLasAlertas = new List<SaldoInventarioDto>();
                if (bodegas != null)
                {
                    foreach (var bodega in bodegas)
                    {
                        try
                        {
                            var alertas = await _inventarioService.GetSaldosConAlertaStockBajoByBodegaAsync(bodega.BodegaID);
                            if (alertas != null)
                            {
                                todasLasAlertas.AddRange(alertas);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error al cargar alertas para bodega {BodegaId}", bodega.BodegaID);
                        }
                    }
                }

                // Cargar movimientos para gráficos
                var todosMovimientos = new List<MovimientoDto>();
                if (bodegas != null)
                {
                    foreach (var bodega in bodegas)
                    {
                        try
                        {
                            var movs = await _movimientoService.GetByBodegaAsync(bodega.BodegaID);
                            if (movs != null)
                            {
                                todosMovimientos.AddRange(movs);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error al cargar movimientos para bodega {BodegaId}", bodega.BodegaID);
                        }
                    }
                }

                // Datos para gráficos
                ViewBag.Solicitudes = solicitudes ?? new List<SolicitudDto>();
                ViewBag.Bodegas = bodegas ?? new List<BodegaDto>();
                ViewBag.Movimientos = todosMovimientos;
                ViewBag.AlertasStock = todasLasAlertas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar estadísticas del dashboard");
                ViewBag.TotalBodegas = 0;
                ViewBag.TotalProductos = 0;
                ViewBag.SolicitudesPendientes = 0;
                ViewBag.UsuariosActivos = 0;
                ViewBag.Solicitudes = new List<SolicitudDto>();
                ViewBag.Bodegas = new List<BodegaDto>();
                ViewBag.AlertasStock = new List<SaldoInventarioDto>();
            }
            
            return View();
        }
    }
}

