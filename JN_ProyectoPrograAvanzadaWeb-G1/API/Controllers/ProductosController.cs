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
    }
}
