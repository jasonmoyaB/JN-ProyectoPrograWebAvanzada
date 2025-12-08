using JN_ProyectoPrograAvanzadaWeb_G1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class BodegasController : Controller
    {
        private readonly IApiBodegaService _bodegaService;
        private readonly ILogger<BodegasController> _logger;

        public BodegasController(IApiBodegaService bodegaService, ILogger<BodegasController> logger)
        {
            _bodegaService = bodegaService;
            _logger = logger;
        }

        // GET: Bodegas
        [HttpGet]
        public async Task<IActionResult> Index(bool? activo = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                var bodegas = await _bodegaService.GetAllAsync(activo);
                ViewBag.FiltroActivo = activo;
                return View(bodegas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodegas");
                TempData["Error"] = "Error al cargar las bodegas";
                return View(new List<BodegaDto>());
            }
        }

        // GET: Bodegas/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            return View(new CrearBodegaDto());
        }

        // POST: Bodegas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearBodegaDto dto)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _bodegaService.CreateAsync(dto);
                TempData["Success"] = "Bodega creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear bodega");
                TempData["Error"] = "Error al crear la bodega";
                return View(dto);
            }
        }

        // GET: Bodegas/Editar
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                var bodega = await _bodegaService.GetByIdAsync(id);
                if (bodega == null)
                {
                    TempData["Error"] = "Bodega no encontrada";
                    return RedirectToAction(nameof(Index));
                }

                var dto = new CrearBodegaDto
                {
                    Nombre = bodega.Nombre,
                    Ubicacion = bodega.Ubicacion
                };

                ViewBag.BodegaID = bodega.BodegaID;
                ViewBag.Activo = bodega.Activo;
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodega: {Id}", id);
                TempData["Error"] = "Error al cargar la bodega";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Bodegas/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, CrearBodegaDto dto)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            if (!ModelState.IsValid)
            {
                var bodega = await _bodegaService.GetByIdAsync(id);
                ViewBag.BodegaID = id;
                ViewBag.Activo = bodega?.Activo ?? false;
                return View(dto);
            }

            try
            {
                var actualizado = await _bodegaService.UpdateAsync(id, dto);
                if (!actualizado)
                {
                    TempData["Error"] = "Error al actualizar la bodega";
                    var bodega = await _bodegaService.GetByIdAsync(id);
                    ViewBag.BodegaID = id;
                    ViewBag.Activo = bodega?.Activo ?? false;
                    return View(dto);
                }

                TempData["Success"] = "Bodega actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar bodega: {Id}", id);
                TempData["Error"] = "Error al actualizar la bodega";
                var bodega = await _bodegaService.GetByIdAsync(id);
                ViewBag.BodegaID = id;
                ViewBag.Activo = bodega?.Activo ?? false;
                return View(dto);
            }
        }

        // POST: Bodegas/ToggleActivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
               
                var existe = await _bodegaService.ExistsAsync(id);
                if (!existe)
                {
                    TempData["Error"] = "La bodega no existe";
                    return RedirectToAction(nameof(Index));
                }

               
                var bodega = await _bodegaService.GetByIdAsync(id);
                if (bodega == null)
                {
                    TempData["Error"] = "No se pudo obtener la información de la bodega";
                    return RedirectToAction(nameof(Index));
                }

                var actualizado = await _bodegaService.ToggleActivoAsync(id);
                if (!actualizado)
                {
                    _logger.LogWarning("ToggleActivoAsync retornó false para bodega ID: {Id}", id);
                    TempData["Error"] = "No se pudo cambiar el estado de la bodega. Verifique que el stored procedure existe y funciona correctamente.";
                }
                else
                {
                    var nuevoEstado = !bodega.Activo ? "activada" : "desactivada";
                    TempData["Success"] = $"Bodega {nuevoEstado} exitosamente";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de bodega: {Id}. Error: {Message}", id, ex.Message);
                TempData["Error"] = $"Error al cambiar el estado de la bodega: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}

