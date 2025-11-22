using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        // GET: Productos
        [HttpGet]
        public async Task<IActionResult> Index(bool? activo = null, string? search = null)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                List<ProductoDto> productos;
                
                if (!string.IsNullOrWhiteSpace(search))
                {
                    productos = await _productoService.SearchAsync(search);
                    if (activo.HasValue)
                    {
                        productos = productos.Where(p => p.Activo == activo.Value).ToList();
                    }
                }
                else
                {
                    productos = await _productoService.GetAllAsync(activo);
                }

                ViewBag.FiltroActivo = activo;
                ViewBag.SearchTerm = search;
                return View(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                TempData["Error"] = "Error al cargar los productos";
                return View(new List<ProductoDto>());
            }
        }

        // GET: Productos/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            return View(new CrearProductoDto());
        }

        // POST: Productos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CrearProductoDto dto)
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
               
                var productoExistente = await _productoService.GetBySkuAsync(dto.SKU);
                if (productoExistente != null)
                {
                    ModelState.AddModelError("SKU", "El SKU ya está registrado");
                    return View(dto);
                }

                await _productoService.CreateAsync(dto);
                TempData["Success"] = "Producto creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                TempData["Error"] = "Error al crear el producto";
                return View(dto);
            }
        }

        // GET: Productos/Editar
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
                var producto = await _productoService.GetByIdAsync(id);
                if (producto == null)
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction(nameof(Index));
                }

                var dto = new CrearProductoDto
                {
                    SKU = producto.SKU,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    EsSerializado = producto.EsSerializado
                };

                ViewBag.ProductoID = producto.ProductoID;
                ViewBag.Activo = producto.Activo;
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto: {Id}", id);
                TempData["Error"] = "Error al cargar el producto";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Productos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, CrearProductoDto dto)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            if (!ModelState.IsValid)
            {
                var producto = await _productoService.GetByIdAsync(id);
                ViewBag.ProductoID = id;
                ViewBag.Activo = producto?.Activo ?? false;
                return View(dto);
            }

            try
            {
                
                var productoExistente = await _productoService.GetBySkuAsync(dto.SKU);
                if (productoExistente != null && productoExistente.ProductoID != id)
                {
                    ModelState.AddModelError("SKU", "El SKU ya está registrado en otro producto");
                    var producto = await _productoService.GetByIdAsync(id);
                    ViewBag.ProductoID = id;
                    ViewBag.Activo = producto?.Activo ?? false;
                    return View(dto);
                }

                var actualizado = await _productoService.UpdateAsync(id, dto);
                if (!actualizado)
                {
                    TempData["Error"] = "Error al actualizar el producto";
                    var producto = await _productoService.GetByIdAsync(id);
                    ViewBag.ProductoID = id;
                    ViewBag.Activo = producto?.Activo ?? false;
                    return View(dto);
                }

                TempData["Success"] = "Producto actualizado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto: {Id}", id);
                TempData["Error"] = "Error al actualizar el producto";
                var producto = await _productoService.GetByIdAsync(id);
                ViewBag.ProductoID = id;
                ViewBag.Activo = producto?.Activo ?? false;
                return View(dto);
            }
        }

        // POST: Productos/ToggleActivo
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
                var actualizado = await _productoService.ToggleActivoAsync(id);
                if (!actualizado)
                {
                    TempData["Error"] = "Error al cambiar el estado del producto";
                }
                else
                {
                    TempData["Success"] = "Estado del producto actualizado";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de producto: {Id}", id);
                TempData["Error"] = "Error al cambiar el estado del producto";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Productos/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var rolId = HttpContext.Session.GetInt32("RolID");
            if (rolId != 1)
            {
                return RedirectToAction("Dashboard", "Tecnico");
            }

            try
            {
                
                var existe = await _productoService.ExistsAsync(id);
                if (!existe)
                {
                    TempData["Error"] = "El producto no existe";
                    return RedirectToAction(nameof(Index));
                }

                var eliminado = await _productoService.DeleteAsync(id);
                if (!eliminado)
                {
                    TempData["Error"] = "No se pudo eliminar el producto. Verifique que no tenga movimientos, series, saldos o solicitudes asociadas.";
                }
                else
                {
                    TempData["Success"] = "Producto eliminado exitosamente";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto: {Id}", id);
                TempData["Error"] = $"Error al eliminar el producto: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

