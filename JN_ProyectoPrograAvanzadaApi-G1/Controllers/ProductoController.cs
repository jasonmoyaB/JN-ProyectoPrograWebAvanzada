using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Data;
using JN_ProyectoPrograAvanzadaApi_G1.Models;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductoController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = "SELECT ProductoID, SKU, Nombre, EsSerializado FROM inv.Productos;";
            using var connection = _connectionFactory.CreateConnection();
            var productos = await connection.QueryAsync<ProductoModel>(sql);
            return Ok(productos);
        }

        [HttpGet("Obtener")]
        public async Task<IActionResult> Obtener()
        {
            using var connection = _connectionFactory.CreateConnection();
            var result = await connection.QueryAsync<ProductoModel>("inv.sp_Producto_GetAll", commandType: System.Data.CommandType.StoredProcedure);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            const string sql = "SELECT ProductoID, SKU, Nombre, EsSerializado FROM inv.Productos WHERE ProductoID = @ProductoID;";
            using var connection = _connectionFactory.CreateConnection();
            var producto = await connection.QuerySingleOrDefaultAsync<ProductoModel>(sql, new { ProductoID = id });
            return producto is null ? NotFound() : Ok(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            const string sql = @"
INSERT INTO inv.Productos (SKU, Nombre, EsSerializado)
VALUES (@SKU, @Nombre, @EsSerializado);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = _connectionFactory.CreateConnection();
            var newId = await connection.ExecuteScalarAsync<int>(sql, request);
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { ProductoID = newId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            const string sql = @"
UPDATE inv.Productos
SET SKU = @SKU,
    Nombre = @Nombre,
    EsSerializado = @EsSerializado
WHERE ProductoID = @ProductoID;";

            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new
            {
                ProductoID = id,
                request.SKU,
                request.Nombre,
                request.EsSerializado
            });

            return affected == 0 ? NotFound() : NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            const string sql = "DELETE FROM inv.Productos WHERE ProductoID = @ProductoID;";
            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { ProductoID = id });
            return affected == 0 ? NotFound() : NoContent();
        }
    }
}
