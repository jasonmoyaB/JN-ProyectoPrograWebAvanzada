using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Data;
using JN_ProyectoPrograAvanzadaApi_G1.Models;
using Microsoft.AspNetCore.Mvc;

namespace JN_ProyectoPrograAvanzadaApi_G1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoController : ControllerBase
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MovimientoController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? bodegaId, [FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
        {
            const string sql = @"
SELECT m.MovimientoID,
       m.BodegaID,
       m.TipoMovimientoID,
       m.UsuarioID,
       m.FechaMovimientoUTC,
       m.Referencia,
       m.Observaciones,
       m.BodegaRelacionadaID,
       m.ProveedorID,
       b.Nombre AS BodegaNombre,
       tm.Codigo AS TipoMovimientoCodigo,
       u.Nombre AS UsuarioNombre
FROM inv.Movimientos m
LEFT JOIN inv.Bodegas b ON m.BodegaID = b.BodegaID
LEFT JOIN inv.TiposMovimiento tm ON m.TipoMovimientoID = tm.TipoMovimientoID
LEFT JOIN inv.Usuarios u ON m.UsuarioID = u.UsuarioID
WHERE (@BodegaID IS NULL OR m.BodegaID = @BodegaID)
  AND (@FechaDesde IS NULL OR m.FechaMovimientoUTC >= @FechaDesde)
  AND (@FechaHasta IS NULL OR m.FechaMovimientoUTC <= @FechaHasta)
ORDER BY m.FechaMovimientoUTC DESC;";

            using var connection = _connectionFactory.CreateConnection();
            var movimientos = await connection.QueryAsync<MovimientoModel>(sql, new
            {
                BodegaID = bodegaId,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta
            });

            return Ok(movimientos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            const string sql = @"
SELECT m.MovimientoID,
       m.BodegaID,
       m.TipoMovimientoID,
       m.UsuarioID,
       m.FechaMovimientoUTC,
       m.Referencia,
       m.Observaciones,
       m.BodegaRelacionadaID,
       m.ProveedorID,
       b.Nombre AS BodegaNombre,
       tm.Codigo AS TipoMovimientoCodigo,
       u.Nombre AS UsuarioNombre
FROM inv.Movimientos m
LEFT JOIN inv.Bodegas b ON m.BodegaID = b.BodegaID
LEFT JOIN inv.TiposMovimiento tm ON m.TipoMovimientoID = tm.TipoMovimientoID
LEFT JOIN inv.Usuarios u ON m.UsuarioID = u.UsuarioID
WHERE m.MovimientoID = @MovimientoID;";

            const string detallesSql = @"
SELECT MovimientoDetalleID,
       MovimientoID,
       ProductoID,
       UbicacionID,
       UnidadID,
       Cantidad,
       CostoUnitario,
       MotivoAjusteID
FROM inv.MovimientoDetalle
WHERE MovimientoID = @MovimientoID;";

            using var connection = _connectionFactory.CreateConnection();
            var movimiento = await connection.QuerySingleOrDefaultAsync<MovimientoModel>(sql, new { MovimientoID = id });
            if (movimiento is null)
            {
                return NotFound();
            }

            var detalles = await connection.QueryAsync<MovimientoDetalleModel>(detallesSql, new { MovimientoID = id });
            return Ok(new { Movimiento = movimiento, Detalles = detalles });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovimientoCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (request.Detalles == null || request.Detalles.Count == 0)
            {
                return BadRequest("Debe incluir al menos un detalle.");
            }

            const string insertMovimiento = @"
INSERT INTO inv.Movimientos
    (BodegaID, TipoMovimientoID, UsuarioID, FechaMovimientoUTC, Referencia, Observaciones, BodegaRelacionadaID, ProveedorID)
VALUES
    (@BodegaID, @TipoMovimientoID, @UsuarioID, @FechaMovimientoUTC, @Referencia, @Observaciones, @BodegaRelacionadaID, @ProveedorID);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            const string insertDetalle = @"
INSERT INTO inv.MovimientoDetalle
    (MovimientoID, ProductoID, UbicacionID, UnidadID, Cantidad, CostoUnitario, MotivoAjusteID)
VALUES
    (@MovimientoID, @ProductoID, @UbicacionID, @UnidadID, @Cantidad, @CostoUnitario, @MotivoAjusteID);";

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var movimientoId = await connection.ExecuteScalarAsync<int>(insertMovimiento, new
                {
                    request.BodegaID,
                    request.TipoMovimientoID,
                    request.UsuarioID,
                    request.FechaMovimientoUTC,
                    request.Referencia,
                    request.Observaciones,
                    request.BodegaRelacionadaID,
                    request.ProveedorID
                }, transaction);

                foreach (var detalle in request.Detalles)
                {
                    await connection.ExecuteAsync(insertDetalle, new
                    {
                        MovimientoID = movimientoId,
                        detalle.ProductoID,
                        detalle.UbicacionID,
                        detalle.UnidadID,
                        detalle.Cantidad,
                        detalle.CostoUnitario,
                        detalle.MotivoAjusteID
                    }, transaction);
                }

                transaction.Commit();
                return CreatedAtAction(nameof(GetById), new { id = movimientoId }, new { MovimientoID = movimientoId });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Problem($"Error al registrar el movimiento: {ex.Message}");
            }
        }
    }
}
