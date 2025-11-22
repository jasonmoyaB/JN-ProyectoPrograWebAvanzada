using Dapper;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public class MovimientoRepository : IMovimientoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MovimientoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Movimiento>> GetByBodegaAsync(int bodegaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var sql = @"
                    SELECT 
                        m.MovimientoID,
                        m.BodegaID,
                        m.TipoMovimientoID,
                        m.UsuarioID,
                        m.FechaMovimientoUTC,
                        m.Referencia,
                        m.Observaciones,
                        m.BodegaRelacionadaID,
                        m.ProveedorID,
                        m.SolicitudID,
                        tm.TipoMovimientoID,
                        tm.Codigo,
                        tm.Nombre,
                        tm.Naturaleza,
                        tm.Activo
                    FROM inv.Movimientos m
                    LEFT JOIN inv.TiposMovimiento tm ON m.TipoMovimientoID = tm.TipoMovimientoID
                    WHERE m.BodegaID = @BodegaID
                        AND (@FechaDesde IS NULL OR m.FechaMovimientoUTC >= @FechaDesde)
                        AND (@FechaHasta IS NULL OR m.FechaMovimientoUTC <= @FechaHasta)
                    ORDER BY m.FechaMovimientoUTC DESC";

                var movimientos = await connection.QueryAsync<Movimiento, TipoMovimiento, Movimiento>(
                    sql,
                    (m, tm) => 
                    { 
                        m.TipoMovimiento = tm ?? new TipoMovimiento { Codigo = "N/A" }; 
                        return m; 
                    },
                    new 
                    { 
                        BodegaID = bodegaId,
                        FechaDesde = fechaDesde,
                        FechaHasta = fechaHasta
                    },
                    splitOn: "TipoMovimientoID");

                var result = movimientos?.ToList() ?? new List<Movimiento>();

                // Cargar relaciones adicionales
                foreach (var movimiento in result)
                {
                    try
                    {
                        // Cargar Usuario
                        if (movimiento.UsuarioID > 0)
                        {
                            movimiento.Usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                                "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                                new { UsuarioID = movimiento.UsuarioID });
                        }
                    }
                    catch
                    {
                        // Si falla cargar el usuario, continuar sin él
                    }
                }

                return result;
            }
            catch
            {
                return new List<Movimiento>();
            }
        }

        public async Task<List<Movimiento>> GetByUsuarioAsync(int usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var sql = @"
                    SELECT 
                        m.MovimientoID,
                        m.BodegaID,
                        m.TipoMovimientoID,
                        m.UsuarioID,
                        m.FechaMovimientoUTC,
                        m.Referencia,
                        m.Observaciones,
                        m.BodegaRelacionadaID,
                        m.ProveedorID,
                        m.SolicitudID,
                        tm.TipoMovimientoID,
                        tm.Codigo,
                        tm.Nombre,
                        tm.Naturaleza,
                        tm.Activo
                    FROM inv.Movimientos m
                    LEFT JOIN inv.TiposMovimiento tm ON m.TipoMovimientoID = tm.TipoMovimientoID
                    WHERE m.UsuarioID = @UsuarioID
                        AND (@FechaDesde IS NULL OR m.FechaMovimientoUTC >= @FechaDesde)
                        AND (@FechaHasta IS NULL OR m.FechaMovimientoUTC <= @FechaHasta)
                    ORDER BY m.FechaMovimientoUTC DESC";

                var movimientos = await connection.QueryAsync<Movimiento, TipoMovimiento, Movimiento>(
                    sql,
                    (m, tm) => 
                    { 
                        m.TipoMovimiento = tm ?? new TipoMovimiento { Codigo = "N/A" }; 
                        return m; 
                    },
                    new 
                    { 
                        UsuarioID = usuarioId,
                        FechaDesde = fechaDesde,
                        FechaHasta = fechaHasta
                    },
                    splitOn: "TipoMovimientoID");

                var result = movimientos?.ToList() ?? new List<Movimiento>();

                // Cargar relaciones adicionales
                foreach (var movimiento in result)
                {
                    try
                    {
                        // Cargar Usuario
                        if (movimiento.UsuarioID > 0)
                        {
                            movimiento.Usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                                "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                                new { UsuarioID = movimiento.UsuarioID });
                        }
                    }
                    catch
                    {
                        // Si falla cargar el usuario, continuar sin él
                    }
                }

                return result;
            }
            catch
            {
                return new List<Movimiento>();
            }
        }

        public async Task<Movimiento?> GetByIdAsync(int movimientoId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT 
                    m.MovimientoID,
                    m.BodegaID,
                    m.TipoMovimientoID,
                    m.UsuarioID,
                    m.FechaMovimientoUTC,
                    m.Referencia,
                    m.Observaciones,
                    m.BodegaRelacionadaID,
                    m.ProveedorID,
                    m.SolicitudID,
                    tm.TipoMovimientoID,
                    tm.Codigo,
                    tm.Nombre,
                    tm.Naturaleza,
                    tm.Activo
                FROM inv.Movimientos m
                LEFT JOIN inv.TiposMovimiento tm ON m.TipoMovimientoID = tm.TipoMovimientoID
                WHERE m.MovimientoID = @MovimientoID";

            var movimiento = await connection.QueryAsync<Movimiento, TipoMovimiento, Movimiento>(
                sql,
                (m, tm) => { m.TipoMovimiento = tm; return m; },
                new { MovimientoID = movimientoId },
                splitOn: "TipoMovimientoID");

            return movimiento.FirstOrDefault();
        }

        public async Task<int> GetCountMovimientosHoyByBodegaAsync(int bodegaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var fechaHoy = DateTime.UtcNow.Date;
            var fechaManana = fechaHoy.AddDays(1);

            var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM inv.Movimientos WHERE BodegaID = @BodegaID AND FechaMovimientoUTC >= @FechaDesde AND FechaMovimientoUTC < @FechaHasta",
                new 
                { 
                    BodegaID = bodegaId,
                    FechaDesde = fechaHoy,
                    FechaHasta = fechaManana
                });

            return count;
        }

        public async Task<int> GetCountMovimientosHoyByUsuarioAsync(int usuarioId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var fechaHoy = DateTime.UtcNow.Date;
            var fechaManana = fechaHoy.AddDays(1);

            var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM inv.Movimientos WHERE UsuarioID = @UsuarioID AND FechaMovimientoUTC >= @FechaDesde AND FechaMovimientoUTC < @FechaHasta",
                new 
                { 
                    UsuarioID = usuarioId,
                    FechaDesde = fechaHoy,
                    FechaHasta = fechaManana
                });

            return count;
        }
    }
}

