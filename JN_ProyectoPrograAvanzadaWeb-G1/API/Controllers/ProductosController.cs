using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos;
using JN_ProyectoPrograAvanzadaWeb_G1.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaWeb_G1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                _logger.LogError(ex, "Error al obtener los productos");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
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
                    return NotFound();
                }
                return Ok(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {ProductoID}", id);
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Create([FromBody] CrearProductoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var productoId = await _productoService.CreateAsyn(dto);
                var producto = await _productoService.GetByIdAsync(productoId);
                return CreatedAtAction(nameof(GetById), new { id = productoId }, producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo producto");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
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
                var existe = await _productoService.ExistsAsync(id);
                if (!existe)
                {
                    return NotFound();
                }
                var actualizar = await _productoService.UpdateAsync(id, dto);
                if (!actualizar)
                {
                    return StatusCode(500, "No se pudo actualizar el producto.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto con ID {ProductoID}", id);
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }
        [HttpPatch("{id}/toggle-activo")]
        public async Task<ActionResult> ToggleActivo(int id)
        {
            try
            {
                var existe = await _productoService.ExistsAsync(id);
                if (!existe)
                {
                    return NotFound(new { message = "Producto no encontrado" });
                }
                var actualizado = await _productoService.ToggleActivoAsync(id);
                if (!actualizado)
                {
                    return StatusCode(500, "No se pudo cambiar el estado del producto.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado del producto con ID {ProductoID}", id);
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }
    }
}
