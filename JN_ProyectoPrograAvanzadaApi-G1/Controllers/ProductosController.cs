using Microsoft.AspNetCore.Mvc;
using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Productos;
using JN_ProyectoPrograAvanzadaApi_G1.Application.Services;
//ProductosController.cs
namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDto>>> GetAll([FromQuery] bool? activo = null)
        {
            try
            {
                var productos = await _productoService.GetAllAsync(activo);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener productos");
                return StatusCode(500, new { message = "Error al obtener productos" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetById(int id)
        {
            try
            {
                var producto = await _productoService.GetByIdAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }
                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto");
                return StatusCode(500, new { message = "Error al obtener producto" });
            }
        }

        [HttpGet("sku/{sku}")]
        public async Task<ActionResult<ProductoDto>> GetBySku(string sku)
        {
            try
            {
                var producto = await _productoService.GetBySkuAsync(sku);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }
                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto por SKU");
                return StatusCode(500, new { message = "Error al obtener producto" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductoDto>>> Search([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new { message = "El término de búsqueda es requerido" });
                }

                var productos = await _productoService.SearchAsync(term);
                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos");
                return StatusCode(500, new { message = "Error al buscar productos" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CrearProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productoId = await _productoService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = productoId }, new { id = productoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear producto");
                return StatusCode(500, new { message = "Error al crear producto" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CrearProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await _productoService.UpdateAsync(id, dto);
                if (!resultado)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }

                return Ok(new { message = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto");
                return StatusCode(500, new { message = "Error al actualizar producto" });
            }
        }

        [HttpPatch("{id}/toggle-activo")]
        public async Task<ActionResult> ToggleActivo(int id)
        {
            try
            {
                var resultado = await _productoService.ToggleActivoAsync(id);
                if (!resultado)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }
                return Ok(new { message = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado del producto");
                return StatusCode(500, new { message = "Error al cambiar estado" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var resultado = await _productoService.DeleteAsync(id);
                if (!resultado)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }
                return Ok(new { message = "Producto eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto");
                return StatusCode(500, new { message = "Error al eliminar producto" });
            }
        }
    }
}

