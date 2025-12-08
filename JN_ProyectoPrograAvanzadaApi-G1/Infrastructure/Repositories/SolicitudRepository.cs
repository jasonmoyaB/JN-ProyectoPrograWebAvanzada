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

                // Cargar relaciones y detalles para cada solicitud
                foreach (var solicitud in result)
                {
                    try
                    {
                        // Cargar Bodega
                        if (solicitud.BodegaID > 0)
                        {
                            solicitud.Bodega = await connection.QueryFirstOrDefaultAsync<Bodega>(
                                "SELECT BodegaID, Nombre, Activo FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                                new { BodegaID = solicitud.BodegaID });
                        }

                        // Cargar Usuario
                        if (solicitud.UsuarioID > 0)
                        {
                            solicitud.Usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                                "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                                new { UsuarioID = solicitud.UsuarioID });
                        }

                        // Cargar detalles
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
                        // Si falla cargar relaciones, continuar sin ellas
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
            
            // Cargar relaciones y detalles
            if (result != null)
            {
                // Cargar Bodega
                var bodega = await connection.QueryFirstOrDefaultAsync<Bodega>(
                    "SELECT BodegaID, Nombre, Activo FROM inv.Bodegas WHERE BodegaID = @BodegaID",
                    new { BodegaID = result.BodegaID });
                result.Bodega = bodega;

                // Cargar Usuario
                var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>(
                    "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                    new { UsuarioID = result.UsuarioID });
                result.Usuario = usuario;

                // Cargar UsuarioAprobador si existe
                if (result.UsuarioAprobadorID.HasValue)
                {
                    var usuarioAprobador = await connection.QueryFirstOrDefaultAsync<Usuario>(
                        "SELECT UsuarioID, Nombre FROM inv.Usuarios WHERE UsuarioID = @UsuarioID",
                        new { UsuarioID = result.UsuarioAprobadorID.Value });
                    result.UsuarioAprobador = usuarioAprobador;
                }

                // Cargar detalles
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

            // Insertar detalles
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

            // Obtener el ID del estado "Pendiente"
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
    }
}

