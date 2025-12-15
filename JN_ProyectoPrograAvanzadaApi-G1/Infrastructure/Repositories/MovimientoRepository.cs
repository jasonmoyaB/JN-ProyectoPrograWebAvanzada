using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
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
                        tm.TipoMovimientoID,
                        tm.Codigo,
                        tm.Naturaleza
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

           
                foreach (var movimiento in result)
                {
                    try
                    {
                        
                        if (movimiento.UsuarioID > 0)
                        {
                            movimiento.Usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                                "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                                new { UsuarioID = movimiento.UsuarioID });
                        }

                        
                        if (movimiento.BodegaID > 0)
                        {
                            movimiento.Bodega = await connection.QueryFirstOrDefaultAsync<Bodega>(
                                "SELECT BodegaID, Nombre FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                                new { BodegaID = movimiento.BodegaID });
                        }

                       
                        if (movimiento.BodegaRelacionadaID.HasValue && movimiento.BodegaRelacionadaID.Value > 0)
                        {
                            movimiento.BodegaRelacionada = await connection.QueryFirstOrDefaultAsync<Bodega>(
                                "SELECT BodegaID, Nombre FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                                new { BodegaID = movimiento.BodegaRelacionadaID.Value });
                        }

                       
                        var detalles = await connection.QueryAsync<MovimientoDetalle, Producto, MovimientoDetalle>(
                            @"SELECT md.MovimientoDetalleID, md.MovimientoID, md.ProductoID, md.Cantidad,
                                     p.ProductoID, p.Nombre, p.SKU
                              FROM inv.MovimientoDetalle md
                              LEFT JOIN inv.Productos p ON md.ProductoID = p.ProductoID
                              WHERE md.MovimientoID = @MovimientoID",
                            (md, p) => { md.Producto = p; return md; },
                            new { MovimientoID = movimiento.MovimientoID },
                            splitOn: "ProductoID");
                        movimiento.Detalles = detalles?.ToList() ?? new List<MovimientoDetalle>();
                    }
                    catch
                    {
                        
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
                        tm.TipoMovimientoID,
                        tm.Codigo,
                        tm.Naturaleza
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

                
                foreach (var movimiento in result)
                {
                    try
                    {
                        
                        if (movimiento.UsuarioID > 0)
                        {
                            movimiento.Usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                                "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                                new { UsuarioID = movimiento.UsuarioID });
                        }

                        
                        if (movimiento.BodegaID > 0)
                        {
                            movimiento.Bodega = await connection.QueryFirstOrDefaultAsync<Bodega>(
                                "SELECT BodegaID, Nombre FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                                new { BodegaID = movimiento.BodegaID });
                        }

                        
                        if (movimiento.BodegaRelacionadaID.HasValue && movimiento.BodegaRelacionadaID.Value > 0)
                        {
                            movimiento.BodegaRelacionada = await connection.QueryFirstOrDefaultAsync<Bodega>(
                                "SELECT BodegaID, Nombre FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                                new { BodegaID = movimiento.BodegaRelacionadaID.Value });
                        }

                        
                        var detalles = await connection.QueryAsync<MovimientoDetalle, Producto, MovimientoDetalle>(
                            @"SELECT md.MovimientoDetalleID, md.MovimientoID, md.ProductoID, md.Cantidad,
                                     p.ProductoID, p.Nombre, p.SKU
                              FROM inv.MovimientoDetalle md
                              LEFT JOIN inv.Productos p ON md.ProductoID = p.ProductoID
                              WHERE md.MovimientoID = @MovimientoID",
                            (md, p) => { md.Producto = p; return md; },
                            new { MovimientoID = movimiento.MovimientoID },
                            splitOn: "ProductoID");
                        movimiento.Detalles = detalles?.ToList() ?? new List<MovimientoDetalle>();
                    }
                    catch
                    {
                        
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
                    tm.TipoMovimientoID,
                    tm.Codigo,
                    tm.Naturaleza
                FROM inv.Movimientos m
                LEFT JOIN inv.TiposMovimiento tm ON m.TipoMovimientoID = tm.TipoMovimientoID
                WHERE m.MovimientoID = @MovimientoID";

            var movimiento = await connection.QueryAsync<Movimiento, TipoMovimiento, Movimiento>(
                sql,
                (m, tm) => { m.TipoMovimiento = tm; return m; },
                new { MovimientoID = movimientoId },
                splitOn: "TipoMovimientoID");

            var result = movimiento.FirstOrDefault();
            if (result != null)
            {
                try
                {
                    
                    if (result.UsuarioID > 0)
                    {
                        result.Usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                            "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                            new { UsuarioID = result.UsuarioID });
                    }

                  
                    if (result.BodegaID > 0)
                    {
                        result.Bodega = await connection.QueryFirstOrDefaultAsync<Bodega>(
                            "SELECT BodegaID, Nombre FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                            new { BodegaID = result.BodegaID });
                    }

                    
                    if (result.BodegaRelacionadaID.HasValue && result.BodegaRelacionadaID.Value > 0)
                    {
                        result.BodegaRelacionada = await connection.QueryFirstOrDefaultAsync<Bodega>(
                            "SELECT BodegaID, Nombre FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                            new { BodegaID = result.BodegaRelacionadaID.Value });
                    }

                    
                    var detalles = await connection.QueryAsync<MovimientoDetalle, Producto, MovimientoDetalle>(
                        @"SELECT md.MovimientoDetalleID, md.MovimientoID, md.ProductoID, md.Cantidad,
                                 p.ProductoID, p.Nombre, p.SKU
                          FROM inv.MovimientoDetalle md
                          LEFT JOIN inv.Productos p ON md.ProductoID = p.ProductoID
                          WHERE md.MovimientoID = @MovimientoID",
                        (md, p) => { md.Producto = p; return md; },
                        new { MovimientoID = result.MovimientoID },
                        splitOn: "ProductoID");
                    result.Detalles = detalles?.ToList() ?? new List<MovimientoDetalle>();
                }
                catch
                {
                    
                }
            }

            return result;
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

        public async Task<int> CreateAsync(Movimiento movimiento, IDbTransaction? transaction = null)
        {
            IDbConnection connection;
            bool shouldDispose = false;

            if (transaction != null)
            {
                connection = transaction.Connection!;
            }
            else
            {
                connection = _connectionFactory.CreateConnection();
                shouldDispose = true;
            }

            try
            {
                var sql = @"
                    INSERT INTO inv.Movimientos (BodegaID, TipoMovimientoID, UsuarioID, FechaMovimientoUTC, Referencia, Observaciones, BodegaRelacionadaID, ProveedorID)
                    VALUES (@BodegaID, @TipoMovimientoID, @UsuarioID, @FechaMovimientoUTC, @Referencia, @Observaciones, @BodegaRelacionadaID, @ProveedorID);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var movimientoId = await connection.QuerySingleAsync<int>(sql, new
                {
                    movimiento.BodegaID,
                    movimiento.TipoMovimientoID,
                    movimiento.UsuarioID,
                    movimiento.FechaMovimientoUTC,
                    movimiento.Referencia,
                    movimiento.Observaciones,
                    movimiento.BodegaRelacionadaID,
                    movimiento.ProveedorID
                }, transaction);

                
                if (movimiento.Detalles != null && movimiento.Detalles.Any())
                {
                    var detalleSql = @"
                        INSERT INTO inv.MovimientoDetalle (MovimientoID, ProductoID, UbicacionID, UnidadID, Cantidad, CostoUnitario, MotivoAjusteID)
                        VALUES (@MovimientoID, @ProductoID, @UbicacionID, @UnidadID, @Cantidad, @CostoUnitario, @MotivoAjusteID);";

                    foreach (var detalle in movimiento.Detalles)
                    {
                        await connection.ExecuteAsync(detalleSql, new
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
                }

                return movimientoId;
            }
            finally
            {
                if (shouldDispose)
                {
                    connection?.Dispose();
                }
            }
        }
    }
}

