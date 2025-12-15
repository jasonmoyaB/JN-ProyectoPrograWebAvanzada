using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Movimientos;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using Dapper;
using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IMovimientoRepository _movimientoRepository;
        private readonly IDbConnectionFactory _connectionFactory;

        public MovimientoService(IMovimientoRepository movimientoRepository, IDbConnectionFactory connectionFactory)
        {
            _movimientoRepository = movimientoRepository;
            _connectionFactory = connectionFactory;
        }

        public async Task<List<MovimientoDto>> GetByBodegaAsync(int bodegaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var movimientos = await _movimientoRepository.GetByBodegaAsync(bodegaId, fechaDesde, fechaHasta);
            return movimientos.Select(m => new MovimientoDto
            {
                MovimientoID = m.MovimientoID,
                BodegaID = m.BodegaID,
                BodegaNombre = m.Bodega?.Nombre ?? string.Empty,
                TipoMovimientoID = m.TipoMovimientoID,
                TipoMovimientoCodigo = m.TipoMovimiento?.Codigo ?? string.Empty,
                TipoMovimientoNombre = m.TipoMovimiento?.Nombre ?? m.TipoMovimiento?.Codigo ?? string.Empty,
                TipoMovimientoNaturaleza = m.TipoMovimiento?.Naturaleza ?? 0,
                UsuarioID = m.UsuarioID,
                UsuarioNombre = m.Usuario?.Nombre ?? string.Empty,
                FechaMovimientoUTC = m.FechaMovimientoUTC,
                Referencia = m.Referencia,
                Observaciones = m.Observaciones,
                BodegaRelacionadaID = m.BodegaRelacionadaID,
                BodegaRelacionadaNombre = m.BodegaRelacionada?.Nombre,
                ProveedorID = m.ProveedorID,
                ProveedorNombre = m.Proveedor?.Nombre,
                Detalles = m.Detalles?.Select(d => new MovimientoDetalleDto
                {
                    MovimientoDetalleID = d.MovimientoDetalleID,
                    ProductoID = d.ProductoID,
                    ProductoNombre = d.Producto?.Nombre ?? string.Empty,
                    ProductoSKU = d.Producto?.SKU ?? string.Empty,
                    Cantidad = d.Cantidad
                }).ToList()
            }).ToList();
        }

        public async Task<List<MovimientoDto>> GetByUsuarioAsync(int usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var movimientos = await _movimientoRepository.GetByUsuarioAsync(usuarioId, fechaDesde, fechaHasta);
            return movimientos.Select(m => new MovimientoDto
            {
                MovimientoID = m.MovimientoID,
                BodegaID = m.BodegaID,
                BodegaNombre = m.Bodega?.Nombre ?? string.Empty,
                TipoMovimientoID = m.TipoMovimientoID,
                TipoMovimientoCodigo = m.TipoMovimiento?.Codigo ?? string.Empty,
                TipoMovimientoNombre = m.TipoMovimiento?.Nombre ?? m.TipoMovimiento?.Codigo ?? string.Empty,
                TipoMovimientoNaturaleza = m.TipoMovimiento?.Naturaleza ?? 0,
                UsuarioID = m.UsuarioID,
                UsuarioNombre = m.Usuario?.Nombre ?? string.Empty,
                FechaMovimientoUTC = m.FechaMovimientoUTC,
                Referencia = m.Referencia,
                Observaciones = m.Observaciones,
                BodegaRelacionadaID = m.BodegaRelacionadaID,
                BodegaRelacionadaNombre = m.BodegaRelacionada?.Nombre,
                ProveedorID = m.ProveedorID,
                ProveedorNombre = m.Proveedor?.Nombre,
                Detalles = m.Detalles?.Select(d => new MovimientoDetalleDto
                {
                    MovimientoDetalleID = d.MovimientoDetalleID,
                    ProductoID = d.ProductoID,
                    ProductoNombre = d.Producto?.Nombre ?? string.Empty,
                    ProductoSKU = d.Producto?.SKU ?? string.Empty,
                    Cantidad = d.Cantidad
                }).ToList()
            }).ToList();
        }

        public async Task<MovimientoDto?> GetByIdAsync(int movimientoId)
        {
            var movimiento = await _movimientoRepository.GetByIdAsync(movimientoId);
            if (movimiento == null) return null;

            return new MovimientoDto
            {
                MovimientoID = movimiento.MovimientoID,
                BodegaID = movimiento.BodegaID,
                BodegaNombre = movimiento.Bodega?.Nombre ?? string.Empty,
                TipoMovimientoID = movimiento.TipoMovimientoID,
                TipoMovimientoCodigo = movimiento.TipoMovimiento?.Codigo ?? string.Empty,
                TipoMovimientoNombre = movimiento.TipoMovimiento?.Nombre ?? movimiento.TipoMovimiento?.Codigo ?? string.Empty,
                TipoMovimientoNaturaleza = movimiento.TipoMovimiento?.Naturaleza ?? 0,
                UsuarioID = movimiento.UsuarioID,
                UsuarioNombre = movimiento.Usuario?.Nombre ?? string.Empty,
                FechaMovimientoUTC = movimiento.FechaMovimientoUTC,
                Referencia = movimiento.Referencia,
                Observaciones = movimiento.Observaciones,
                BodegaRelacionadaID = movimiento.BodegaRelacionadaID,
                BodegaRelacionadaNombre = movimiento.BodegaRelacionada?.Nombre,
                ProveedorID = movimiento.ProveedorID,
                ProveedorNombre = movimiento.Proveedor?.Nombre,
                Detalles = movimiento.Detalles?.Select(d => new MovimientoDetalleDto
                {
                    MovimientoDetalleID = d.MovimientoDetalleID,
                    ProductoID = d.ProductoID,
                    ProductoNombre = d.Producto?.Nombre ?? string.Empty,
                    ProductoSKU = d.Producto?.SKU ?? string.Empty,
                    Cantidad = d.Cantidad
                }).ToList()
            };
        }

        public async Task<int> GetCountMovimientosHoyByBodegaAsync(int bodegaId)
        {
            return await _movimientoRepository.GetCountMovimientosHoyByBodegaAsync(bodegaId);
        }

        public async Task<int> GetCountMovimientosHoyByUsuarioAsync(int usuarioId)
        {
            return await _movimientoRepository.GetCountMovimientosHoyByUsuarioAsync(usuarioId);
        }

        public async Task<int> CreateTrasladoAsync(MovimientoTrasladoDto dto, int usuarioId)
        {
            var connection = _connectionFactory.CreateConnection();
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }
                using var transaction = connection.BeginTransaction();

                try
                {
                    // Obtener o crear tipos de movimiento
                    var tipoSalida = await GetOrCreateTipoMovimientoAsync(connection, transaction, "SALIDA", -1);
                    var tipoEntrada = await GetOrCreateTipoMovimientoAsync(connection, transaction, "ENTRADA", 1);

                    // Obtener o crear unidad de medida por defecto
                    var unidadPorDefecto = await GetOrCreateUnidadPorDefectoAsync(connection, transaction);

                    // Generar referencia si no se proporciona
                    var referencia = dto.Referencia ?? $"TRAS-{DateTime.UtcNow:yyyyMMddHHmmss}";

                    // Crear movimiento de SALIDA desde bodega origen
                    var movimientoSalida = new Movimiento
                    {
                        BodegaID = dto.BodegaOrigenID,
                        TipoMovimientoID = tipoSalida.TipoMovimientoID,
                        UsuarioID = usuarioId,
                        FechaMovimientoUTC = DateTime.UtcNow,
                        Referencia = referencia,
                        Observaciones = dto.Observaciones,
                        BodegaRelacionadaID = dto.BodegaDestinoID,
                        Detalles = dto.Detalles.Select(d => new MovimientoDetalle
                        {
                            ProductoID = d.ProductoID,
                            UnidadID = d.UnidadID > 0 ? d.UnidadID : unidadPorDefecto.UnidadID,
                            Cantidad = d.Cantidad,
                            CostoUnitario = d.CostoUnitario,
                            UbicacionID = d.UbicacionID,
                            MotivoAjusteID = d.MotivoAjusteID
                        }).ToList()
                    };

                    var movimientoSalidaId = await _movimientoRepository.CreateAsync(movimientoSalida, transaction);

                    // Crear movimiento de ENTRADA a bodega destino
                    var movimientoEntrada = new Movimiento
                    {
                        BodegaID = dto.BodegaDestinoID,
                        TipoMovimientoID = tipoEntrada.TipoMovimientoID,
                        UsuarioID = usuarioId,
                        FechaMovimientoUTC = DateTime.UtcNow,
                        Referencia = referencia,
                        Observaciones = dto.Observaciones,
                        BodegaRelacionadaID = dto.BodegaOrigenID,
                        Detalles = dto.Detalles.Select(d => new MovimientoDetalle
                        {
                            ProductoID = d.ProductoID,
                            UnidadID = d.UnidadID > 0 ? d.UnidadID : unidadPorDefecto.UnidadID,
                            Cantidad = d.Cantidad,
                            CostoUnitario = d.CostoUnitario,
                            UbicacionID = d.UbicacionID,
                            MotivoAjusteID = d.MotivoAjusteID
                        }).ToList()
                    };

                    var movimientoEntradaId = await _movimientoRepository.CreateAsync(movimientoEntrada, transaction);

                    transaction.Commit();
                    return movimientoSalidaId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }
        }

        private async Task<TipoMovimiento> GetOrCreateTipoMovimientoAsync(IDbConnection connection, IDbTransaction transaction, string codigo, short naturaleza)
        {
            var tipo = await connection.QueryFirstOrDefaultAsync<TipoMovimiento>(
                "SELECT TipoMovimientoID, Codigo, Naturaleza FROM inv.TiposMovimiento WHERE Codigo = @Codigo",
                new { Codigo = codigo },
                transaction);

            if (tipo != null)
            {
                return tipo;
            }

            // Crear tipo de movimiento si no existe
            var sql = @"
                INSERT INTO inv.TiposMovimiento (Codigo, Naturaleza)
                VALUES (@Codigo, @Naturaleza);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var tipoId = await connection.QuerySingleAsync<int>(sql, new
            {
                Codigo = codigo,
                Naturaleza = naturaleza
            }, transaction);

            return new TipoMovimiento
            {
                TipoMovimientoID = tipoId,
                Codigo = codigo,
                Naturaleza = naturaleza
            };
        }

        private async Task<UnidadMedida> GetOrCreateUnidadPorDefectoAsync(IDbConnection connection, IDbTransaction transaction)
        {
            var unidad = await connection.QueryFirstOrDefaultAsync<UnidadMedida>(
                "SELECT UnidadID, Nombre FROM inv.UnidadesMedida WHERE Nombre = 'Unidad'",
                transaction: transaction);

            if (unidad != null)
            {
                return unidad;
            }

            
            var sql = @"
                INSERT INTO inv.UnidadesMedida (Nombre)
                VALUES ('Unidad');
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var unidadId = await connection.QuerySingleAsync<int>(sql, transaction: transaction);

            return new UnidadMedida
            {
                UnidadID = unidadId,
                Nombre = "Unidad"
            };
        }
    }
}

