using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Data;
using JN_ProyectoPrograAvanzadaApi_G1.Models;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AutenticacionController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            const string sql = @"
SELECT TOP 1 u.UsuarioID,
             u.Nombre,
             u.CorreoElectronico,
             u.ContrasenaHash,
             u.RolID,
             u.Activo,
             r.NombreRol
FROM inv.Usuarios u
INNER JOIN inv.Roles r ON u.RolID = r.RolID
WHERE LOWER(u.CorreoElectronico) = LOWER(@CorreoElectronico);";

            using var connection = _connectionFactory.CreateConnection();
            var usuario = await connection.QuerySingleOrDefaultAsync<UsuarioModel>(sql, new
            {
                request.CorreoElectronico
            });

            if (usuario == null || string.IsNullOrWhiteSpace(usuario.ContrasenaHash))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            if (!usuario.Activo)
            {
                return Forbid("El usuario está inactivo.");
            }

            if (!string.Equals(usuario.ContrasenaHash, request.Contrasena, StringComparison.Ordinal))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            var response = new LoginResponse
            {
                UsuarioID = usuario.UsuarioID,
                Nombre = usuario.Nombre,
                CorreoElectronico = usuario.CorreoElectronico ?? string.Empty,
                RolID = usuario.RolID,
                RolNombre = usuario.RolNombre ?? string.Empty,
                Activo = usuario.Activo
            };

            return Ok(response);
        }

        [HttpPost("registrar-tecnico")]
        public async Task<IActionResult> RegistrarTecnico([FromBody] UsuarioCreateRequest request)
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
            var nuevoId = await connection.ExecuteScalarAsync<int>(sql, new
            {
                request.Nombre,
                request.CorreoElectronico,
                ContrasenaHash = request.Contrasena ?? "InvTech123!",
                RolID = 2,
                request.Activo
            });

            return CreatedAtAction(nameof(Login), new { id = nuevoId }, new { UsuarioID = nuevoId });
        }
    }
}
