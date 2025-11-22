using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Data;
using JN_ProyectoPrograAvanzadaApi_G1.Models;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = @"
SELECT u.UsuarioID,
       u.Nombre,
       u.CorreoElectronico,
       u.RolID,
       u.Activo,
       r.NombreRol
FROM inv.Usuarios u
LEFT JOIN inv.Roles r ON u.RolID = r.RolID;";

            using var connection = _connectionFactory.CreateConnection();
            var usuarios = await connection.QueryAsync<UsuarioModel>(sql);
            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            const string sql = @"
SELECT u.UsuarioID,
       u.Nombre,
       u.CorreoElectronico,
       u.RolID,
       u.Activo,
       r.NombreRol
FROM inv.Usuarios u
LEFT JOIN inv.Roles r ON u.RolID = r.RolID
WHERE u.UsuarioID = @UsuarioID;";

            using var connection = _connectionFactory.CreateConnection();
            var usuario = await connection.QuerySingleOrDefaultAsync<UsuarioModel>(sql, new { UsuarioID = id });
            return usuario is null ? NotFound() : Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            const string sql = @"
INSERT INTO inv.Usuarios (Nombre, CorreoElectronico, ContrasenaHash, RolID, Activo)
VALUES (@Nombre, @CorreoElectronico, @ContrasenaHash, @RolID, @Activo);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = _connectionFactory.CreateConnection();
            var newId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                request.Nombre,
                request.CorreoElectronico,
                ContrasenaHash = request.Contrasena ?? "InvTech123!",
                request.RolID,
                request.Activo
            });

            return CreatedAtAction(nameof(GetById), new { id = newId }, new { UsuarioID = newId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var sql = @"
UPDATE inv.Usuarios
SET Nombre = @Nombre,
    CorreoElectronico = @CorreoElectronico,
    RolID = @RolID,
    Activo = @Activo" +
    (string.IsNullOrWhiteSpace(request.Contrasena) ? string.Empty : ", ContrasenaHash = @ContrasenaHash") +
@" WHERE UsuarioID = @UsuarioID;";

            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new
            {
                UsuarioID = id,
                request.Nombre,
                request.CorreoElectronico,
                request.RolID,
                request.Activo,
                ContrasenaHash = request.Contrasena
            });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            const string sql = "DELETE FROM inv.Usuarios WHERE UsuarioID = @UsuarioID;";

            using var connection = _connectionFactory.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { UsuarioID = id });

            if (affected == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
