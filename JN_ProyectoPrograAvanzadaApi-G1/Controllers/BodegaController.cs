using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Data;
using JN_ProyectoPrograAvanzadaApi_G1.Models;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodegaController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BodegaController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = "SELECT BodegaID, Nombre, Activo FROM inv.Bodegas;";
            using var connection = _connectionFactory.CreateConnection();
            var bodegas = await connection.QueryAsync<BodegaModel>(sql);
            return Ok(bodegas);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            const string sql = "SELECT BodegaID, Nombre, Activo FROM inv.Bodegas WHERE BodegaID = @BodegaID;";
            using var connection = _connectionFactory.CreateConnection();
            var bodega = await connection.QuerySingleOrDefaultAsync<BodegaModel>(sql, new { BodegaID = id });
            return bodega is null ? NotFound() : Ok(bodega);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BodegaModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            const string sql = @"INSERT INTO inv.Bodegas (Nombre, Activo) VALUES (@Nombre, @Activo);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _connectionFactory.CreateConnection();
            var newId = await connection.ExecuteScalarAsync<int>(sql, new { request.Nombre, request.Activo });
            return CreatedAtAction(nameof(GetById), new { id = newId }, new { BodegaID = newId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BodegaModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            const string sql = @"UPDATE inv.Bodegas SET Nombre = @Nombre, Activo = @Activo WHERE BodegaID = @BodegaID;";
            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { BodegaID = id, request.Nombre, request.Activo });
            return affected == 0 ? NotFound() : NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            const string sql = @"DELETE FROM inv.Bodegas WHERE BodegaID = @BodegaID;";
            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { BodegaID = id });
            return affected == 0 ? NotFound() : NoContent();
        }
    }
}
