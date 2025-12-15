using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public class SolicitudRepository : ISolicitudRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SolicitudRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Solicitud>> GetAllAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var sql = @"
                    SELECT 
                        s.SolicitudID,
                        s.EstadoSolicitudID,
                        s.BodegaID,
                        s.UsuarioID,
                        s.FechaCreacionUTC,
                        s.FechaAprobacionUTC,
                        s.FechaEnvioUTC,
                        s.FechaEntregaUTC,
                        s.Comentarios,
                        s.UsuarioAprobadorID,
                        s.DespachoID,
                        es.EstadoSolicitudID,
                        es.Codigo
                    FROM inv.Solicitudes s
                    LEFT JOIN inv.EstadosSolicitud es ON s.EstadoSolicitudID = es.EstadoSolicitudID
                    ORDER BY s.FechaCreacionUTC DESC";

                var solicitudes = await connection.QueryAsync<Solicitud, EstadoSolicitud, Solicitud>(
                    sql,
                    (s, es) => 
                    { 
                        s.EstadoSolicitud = es ?? new EstadoSolicitud { Codigo = "N/A" }; 
                        return s; 
                    },
                    splitOn: "EstadoSolicitudID");

                var result = solicitudes?.ToList() ?? new List<Solicitud>();

                
                foreach (var solicitud in result)
                {
                    try
                    {
                       
                        if (solicitud.BodegaID > 0)
                        {
                            solicitud.Bodega = await connection.QueryFirstOrDefaultAsync<Bodega>(
                                "SELECT BodegaID, Nombre, Activo FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                                new { BodegaID = solicitud.BodegaID });
                        }

                        
                        if (solicitud.UsuarioID > 0)
                        {
                            solicitud.Usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                                "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                                new { UsuarioID = solicitud.UsuarioID });
                        }

                        
                        if (solicitud.UsuarioAprobadorID.HasValue && solicitud.UsuarioAprobadorID.Value > 0)
                        {
                            solicitud.UsuarioAprobador = await connection.QueryFirstOrDefaultAsync<Usuario>(
                                "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                                new { UsuarioID = solicitud.UsuarioAprobadorID.Value });
                        }

                        
                        var detallesSql = @"
                            SELECT 
                                sd.SolicitudDetalleID,
                                sd.SolicitudID,
                                sd.ProductoID,
                                sd.CantidadSolicitada,
                                sd.CantidadEnviada,
                                p.ProductoID,
                                p.SKU,
                                p.Nombre
                            FROM inv.SolicitudDetalle sd
                            LEFT JOIN inv.Productos p ON sd.ProductoID = p.ProductoID
                            WHERE sd.SolicitudID = @SolicitudID";

                        var detalles = await connection.QueryAsync<SolicitudDetalle, Producto, SolicitudDetalle>(
                            detallesSql,
                            (sd, p) => { sd.Producto = p; return sd; },
                            new { SolicitudID = solicitud.SolicitudID },
                            splitOn: "ProductoID");

                        solicitud.Detalles = detalles?.ToList() ?? new List<SolicitudDetalle>();
                    }
                    catch
                    {
                       
                        solicitud.Detalles = new List<SolicitudDetalle>();
                    }
                }

                return result;
            }
            catch
            {
                return new List<Solicitud>();
            }
        }

        public async Task<List<Solicitud>> GetByUsuarioAsync(int usuarioId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var sql = @"
                    SELECT 
                        s.SolicitudID,
                        s.EstadoSolicitudID,
                        s.BodegaID,
                        s.UsuarioID,
                        s.FechaCreacionUTC,
                        s.FechaAprobacionUTC,
                        s.FechaEnvioUTC,
                        s.FechaEntregaUTC,
                        s.Comentarios,
                        s.UsuarioAprobadorID,
                        s.DespachoID,
                        es.EstadoSolicitudID,
                        es.Codigo
                    FROM inv.Solicitudes s
                    LEFT JOIN inv.EstadosSolicitud es ON s.EstadoSolicitudID = es.EstadoSolicitudID
                    WHERE s.UsuarioID = @UsuarioID
                    ORDER BY s.FechaCreacionUTC DESC";

                var solicitudes = await connection.QueryAsync<Solicitud, EstadoSolicitud, Solicitud>(
                    sql,
                    (s, es) => 
                    { 
                        s.EstadoSolicitud = es ?? new EstadoSolicitud { Codigo = "N/A" }; 
                        return s; 
                    },
                    new { UsuarioID = usuarioId },
                    splitOn: "EstadoSolicitudID");

                var result = solicitudes?.ToList() ?? new List<Solicitud>();

               
                foreach (var solicitud in result)
                {
                    try
                    {
                        
                        if (solicitud.BodegaID > 0)
                        {
                            solicitud.Bodega = await connection.QueryFirstOrDefaultAsync<Bodega>(
                                "SELECT BodegaID, Nombre, Activo FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                                new { BodegaID = solicitud.BodegaID });
                        }

                       
                        if (solicitud.UsuarioID > 0)
                        {
                            solicitud.Usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                                "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                                new { UsuarioID = solicitud.UsuarioID });
                        }

                        
                        var detallesSql = @"
                            SELECT 
                                sd.SolicitudDetalleID,
                                sd.SolicitudID,
                                sd.ProductoID,
                                sd.CantidadSolicitada,
                                sd.CantidadEnviada,
                                p.ProductoID,
                                p.SKU,
                                p.Nombre
                            FROM inv.SolicitudDetalle sd
                            LEFT JOIN inv.Productos p ON sd.ProductoID = p.ProductoID
                            WHERE sd.SolicitudID = @SolicitudID";

                        var detalles = await connection.QueryAsync<SolicitudDetalle, Producto, SolicitudDetalle>(
                            detallesSql,
                            (sd, p) => { sd.Producto = p; return sd; },
                            new { SolicitudID = solicitud.SolicitudID },
                            splitOn: "ProductoID");

                        solicitud.Detalles = detalles?.ToList() ?? new List<SolicitudDetalle>();
                    }
                    catch
                    {
                        
                        solicitud.Detalles = new List<SolicitudDetalle>();
                    }
                }

                return result;
            }
            catch
            {
                return new List<Solicitud>();
            }
        }

        public async Task<List<Solicitud>> GetByBodegaAsync(int bodegaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT 
                    s.SolicitudID,
                    s.EstadoSolicitudID,
                    s.BodegaID,
                    s.UsuarioID,
                    s.FechaCreacionUTC,
                    s.FechaAprobacionUTC,
                    s.FechaEnvioUTC,
                    s.FechaEntregaUTC,
                    s.Comentarios,
                    s.UsuarioAprobadorID,
                    s.DespachoID,
                    es.EstadoSolicitudID,
                    es.Codigo
                FROM inv.Solicitudes s
                LEFT JOIN inv.EstadosSolicitud es ON s.EstadoSolicitudID = es.EstadoSolicitudID
                WHERE s.BodegaID = @BodegaID
                ORDER BY s.FechaCreacionUTC DESC";

            var solicitudes = await connection.QueryAsync<Solicitud, EstadoSolicitud, Solicitud>(
                sql,
                (s, es) => { s.EstadoSolicitud = es; return s; },
                new { BodegaID = bodegaId },
                splitOn: "EstadoSolicitudID");

            return solicitudes.ToList();
        }

        public async Task<Solicitud?> GetByIdAsync(int solicitudId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT 
                    s.SolicitudID,
                    s.EstadoSolicitudID,
                    s.BodegaID,
                    s.UsuarioID,
                    s.FechaCreacionUTC,
                    s.FechaAprobacionUTC,
                    s.FechaEnvioUTC,
                    s.FechaEntregaUTC,
                    s.Comentarios,
                    s.UsuarioAprobadorID,
                    s.DespachoID,
                    es.EstadoSolicitudID,
                    es.Codigo
                FROM inv.Solicitudes s
                LEFT JOIN inv.EstadosSolicitud es ON s.EstadoSolicitudID = es.EstadoSolicitudID
                WHERE s.SolicitudID = @SolicitudID";

            var solicitud = await connection.QueryAsync<Solicitud, EstadoSolicitud, Solicitud>(
                sql,
                (s, es) => { s.EstadoSolicitud = es; return s; },
                new { SolicitudID = solicitudId },
                splitOn: "EstadoSolicitudID");

            var result = solicitud.FirstOrDefault();
           
            if (result != null)
            {
               
                var bodega = await connection.QueryFirstOrDefaultAsync<Bodega>(
                    "SELECT BodegaID, Nombre, Activo FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                    new { BodegaID = result.BodegaID });
                result.Bodega = bodega;

                
                var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                    new { UsuarioID = result.UsuarioID });
                result.Usuario = usuario;

               
                if (result.UsuarioAprobadorID.HasValue)
                {
                    var usuarioAprobador = await connection.QueryFirstOrDefaultAsync<Usuario>(
                        "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                        new { UsuarioID = result.UsuarioAprobadorID.Value });
                    result.UsuarioAprobador = usuarioAprobador;
                }

                
                var detallesSql = @"
                    SELECT 
                        sd.SolicitudDetalleID,
                        sd.SolicitudID,
                        sd.ProductoID,
                        sd.CantidadSolicitada,
                        sd.CantidadEnviada,
                        p.ProductoID,
                        p.SKU,
                        p.Nombre
                    FROM inv.SolicitudDetalle sd
                    LEFT JOIN inv.Productos p ON sd.ProductoID = p.ProductoID
                    WHERE sd.SolicitudID = @SolicitudID";

                var detalles = await connection.QueryAsync<SolicitudDetalle, Producto, SolicitudDetalle>(
                    detallesSql,
                    (sd, p) => { sd.Producto = p; return sd; },
                    new { SolicitudID = solicitudId },
                    splitOn: "ProductoID");

                result.Detalles = detalles.ToList();
            }

            return result;
        }

        public async Task<int> CreateAsync(Solicitud solicitud)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                INSERT INTO inv.Solicitudes (EstadoSolicitudID, BodegaID, UsuarioID, FechaCreacionUTC, Comentarios)
                VALUES (@EstadoSolicitudID, @BodegaID, @UsuarioID, @FechaCreacionUTC, @Comentarios);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var solicitudId = await connection.QuerySingleAsync<int>(sql, new
            {
                solicitud.EstadoSolicitudID,
                solicitud.BodegaID,
                solicitud.UsuarioID,
                solicitud.FechaCreacionUTC,
                solicitud.Comentarios
            });

           
            if (solicitud.Detalles != null && solicitud.Detalles.Any())
            {
                var detalleSql = @"
                    INSERT INTO inv.SolicitudDetalle (SolicitudID, ProductoID, CantidadSolicitada)
                    VALUES (@SolicitudID, @ProductoID, @CantidadSolicitada);";

                foreach (var detalle in solicitud.Detalles)
                {
                    await connection.ExecuteAsync(detalleSql, new
                    {
                        SolicitudID = solicitudId,
                        detalle.ProductoID,
                        detalle.CantidadSolicitada
                    });
                }
            }

            return solicitudId;
        }

        public async Task<int> GetCountPendientesByUsuarioAsync(int usuarioId)
        {
            using var connection = _connectionFactory.CreateConnection();

       
            var estadoPendienteId = await connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT EstadoSolicitudID FROM inv.EstadosSolicitud WHERE Codigo = 'Pendiente'");

            if (!estadoPendienteId.HasValue)
                return 0;

            var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM inv.Solicitudes WHERE UsuarioID = @UsuarioID AND EstadoSolicitudID = @EstadoSolicitudID",
                new 
                { 
                    UsuarioID = usuarioId,
                    EstadoSolicitudID = estadoPendienteId.Value
                });

            return count;
        }

        public async Task<bool> UpdateEstadoAsync(int solicitudId, int estadoSolicitudId, int? usuarioAprobadorId = null, DateTime? fechaAprobacion = null, DateTime? fechaEnvio = null, DateTime? fechaEntrega = null, string? comentarios = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            var sql = @"
                UPDATE inv.Solicitudes 
                SET EstadoSolicitudID = @EstadoSolicitudID,
                    UsuarioAprobadorID = @UsuarioAprobadorID,
                    FechaAprobacionUTC = @FechaAprobacionUTC,
                    FechaEnvioUTC = @FechaEnvioUTC,
                    FechaEntregaUTC = @FechaEntregaUTC,
                    Comentarios = CASE 
                        WHEN @Comentarios IS NOT NULL AND @Comentarios != '' 
                        THEN ISNULL(Comentarios + CHAR(13) + CHAR(10) + @Comentarios, @Comentarios)
                        ELSE Comentarios
                    END
                WHERE SolicitudID = @SolicitudID";

            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                SolicitudID = solicitudId,
                EstadoSolicitudID = estadoSolicitudId,
                UsuarioAprobadorID = usuarioAprobadorId,
                FechaAprobacionUTC = fechaAprobacion,
                FechaEnvioUTC = fechaEnvio,
                FechaEntregaUTC = fechaEntrega,
                Comentarios = comentarios
            });

            return rowsAffected > 0;
        }

        public async Task<bool> EstablecerCantidadesEnviadasAsync(int solicitudId, Dictionary<int, decimal> cantidades)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                foreach (var kvp in cantidades)
                {
                    var sql = @"
                        UPDATE inv.SolicitudDetalle 
                        SET CantidadEnviada = @CantidadEnviada
                        WHERE SolicitudID = @SolicitudID AND ProductoID = @ProductoID";

                    await connection.ExecuteAsync(sql, new
                    {
                        SolicitudID = solicitudId,
                        ProductoID = kvp.Key,
                        CantidadEnviada = kvp.Value
                    }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> TieneCantidadesEnviadasAsync(int solicitudId)
        {
            using var connection = _connectionFactory.CreateConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            var sql = @"
                SELECT COUNT(1) 
                FROM inv.SolicitudDetalle 
                WHERE SolicitudID = @SolicitudID AND CantidadEnviada IS NOT NULL";

            var count = await connection.QueryFirstOrDefaultAsync<int>(sql, new { SolicitudID = solicitudId });
            return count > 0;
        }
    }
}

