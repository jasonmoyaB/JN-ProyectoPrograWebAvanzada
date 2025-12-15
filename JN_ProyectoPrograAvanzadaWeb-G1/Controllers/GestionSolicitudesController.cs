using JN_ProyectoPrograAvanzadaWeb_G1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class GestionSolicitudesController : Controller
    {
        private readonly IApiSolicitudService _solicitudService;
        private readonly IApiInventarioService _inventarioService;
        private readonly ILogger<GestionSolicitudesController> _logger;

        public GestionSolicitudesController(
            IApiSolicitudService solicitudService,
            IApiInventarioService inventarioService,
            ILogger<GestionSolicitudesController> logger)
        {
            _solicitudService = solicitudService;
            _inventarioService = inventarioService;
            _logger = logger;
        }

        // GET: GestionSolicitudes
        [HttpGet]
        public async Task<IActionResult> Index(string? estado = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                
                var solicitudes = await _solicitudService.GetAllAsync();
                
                
                if (!string.IsNullOrEmpty(estado))
                {
                    solicitudes = solicitudes?.Where(s => s.EstadoCodigo == estado).ToList();
                }
                
                ViewBag.EstadoFiltro = estado;
                return View(solicitudes ?? new List<SolicitudDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar solicitudes");
                TempData["Error"] = "Error al cargar las solicitudes: " + ex.Message;
                return View(new List<SolicitudDto>());
            }
        }

        // GET: GestionSolicitudes/Detalles
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
                var solicitud = await _solicitudService.GetByIdAsync(id);
                if (solicitud == null)
                {
                    TempData["Error"] = "Solicitud no encontrada";
                    return RedirectToAction(nameof(Index));
                }

               
                var stockBodegaGeneral = new Dictionary<int, decimal>();
                if (solicitud.Detalles != null)
                {
                    foreach (var detalle in solicitud.Detalles)
                    {
                        var saldo = await _inventarioService.GetSaldoByBodegaAndProductoAsync(1, detalle.ProductoID);
                        stockBodegaGeneral[detalle.ProductoID] = saldo?.Cantidad ?? 0;
                    }
                }

                ViewBag.StockBodegaGeneral = stockBodegaGeneral;
                ViewBag.TieneCantidadesEnviadas = await _solicitudService.TieneCantidadesEnviadasAsync(id);

                return View(solicitud);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar detalle de solicitud");
                TempData["Error"] = "Error al cargar la solicitud";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: GestionSolicitudes/Aprobar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprobar(int id, string? comentarios = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            if (!usuarioId.HasValue)
            {
                TempData["Error"] = "Error al identificar al usuario";
                return RedirectToAction(nameof(Detalles), new { id });
            }

            try
            {
                await _solicitudService.AprobarAsync(id, usuarioId.Value, comentarios);
                TempData["Success"] = "Solicitud aprobada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aprobar solicitud {Id}", id);
                TempData["Error"] = $"Error al aprobar la solicitud: {ex.Message}";
            }

            return RedirectToAction(nameof(Detalles), new { id });
        }

        // POST: GestionSolicitudes/Rechazar/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int id, string? comentarios = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            if (!usuarioId.HasValue)
            {
                TempData["Error"] = "Error al identificar al usuario";
                return RedirectToAction(nameof(Detalles), new { id });
            }

            try
            {
                await _solicitudService.RechazarAsync(id, usuarioId.Value, comentarios);
                TempData["Success"] = "Solicitud rechazada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al rechazar solicitud {Id}", id);
                TempData["Error"] = $"Error al rechazar la solicitud: {ex.Message}";
            }

            return RedirectToAction(nameof(Detalles), new { id });
        }

        // POST: GestionSolicitudes/Enviar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(int id, string? comentarios = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                await _solicitudService.EnviarAsync(id, comentarios);
                TempData["Success"] = "Solicitud enviada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar solicitud {Id}", id);
                TempData["Error"] = $"Error al enviar la solicitud: {ex.Message}";
            }

            return RedirectToAction(nameof(Detalles), new { id });
        }

        // POST: GestionSolicitudes/Entregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entregar(int id, string? comentarios = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            if (!usuarioId.HasValue)
            {
                TempData["Error"] = "Error al identificar al usuario";
                return RedirectToAction(nameof(Detalles), new { id });
            }

            try
            {
                
                var solicitud = await _solicitudService.GetByIdAsync(id);
                if (solicitud == null)
                {
                    TempData["Error"] = "Solicitud no encontrada";
                    return RedirectToAction(nameof(Index));
                }

               
                await _solicitudService.EntregarAsync(id, 1, solicitud.BodegaID, usuarioId.Value, comentarios);
                TempData["Success"] = "Solicitud marcada como entregada exitosamente. Los movimientos de inventario han sido creados.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al entregar solicitud {Id}", id);
                TempData["Error"] = $"Error al marcar como entregada la solicitud: {ex.Message}";
            }

            return RedirectToAction(nameof(Detalles), new { id });
        }

        // POST: GestionSolicitudes/EstablecerCantidadesEnviadas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EstablecerCantidadesEnviadas()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            
            if (!int.TryParse(Request.Form["id"], out int id) || id <= 0)
            {
                TempData["Error"] = "ID de solicitud inválido";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                
                var cantidades = new Dictionary<int, decimal>();
                
                
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("cantidades_"))
                    {
                       
                        var productoIdStr = key.Substring("cantidades_".Length);
                        if (int.TryParse(productoIdStr, out int productoId))
                        {
                            if (decimal.TryParse(Request.Form[key], out decimal cantidad))
                            {
                                cantidades[productoId] = cantidad;
                                _logger.LogInformation("Cantidad encontrada: ProductoID={ProductoID}, Cantidad={Cantidad}", productoId, cantidad);
                            }
                        }
                    }
                }

                if (cantidades.Count == 0)
                {
                    _logger.LogWarning("No se encontraron cantidades en el formulario. Claves recibidas: {Keys}", string.Join(", ", Request.Form.Keys));
                    TempData["Error"] = "No se recibieron cantidades para establecer. Por favor, asegúrese de ingresar valores en los campos de cantidad.";
                    return RedirectToAction(nameof(Detalles), new { id });
                }

                var dto = new EstablecerCantidadesEnviadasDto
                {
                    Detalles = cantidades.Select(kvp => new CantidadEnviadaDetalleDto
                    {
                        ProductoID = kvp.Key,
                        CantidadEnviada = kvp.Value
                    }).ToList()
                };

                await _solicitudService.EstablecerCantidadesEnviadasAsync(id, dto);
                TempData["Success"] = "Cantidades a enviar establecidas exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer cantidades enviadas para solicitud {Id}", id);
                TempData["Error"] = $"Error al establecer las cantidades: {ex.Message}";
            }

            return RedirectToAction(nameof(Detalles), new { id });
        }
    }
}
