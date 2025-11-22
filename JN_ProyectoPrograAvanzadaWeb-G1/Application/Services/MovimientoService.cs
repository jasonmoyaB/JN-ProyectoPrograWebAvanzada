using JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Movimientos;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IMovimientoRepository _movimientoRepository;

        public MovimientoService(IMovimientoRepository movimientoRepository)
        {
            _movimientoRepository = movimientoRepository;
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
                UsuarioID = m.UsuarioID,
                UsuarioNombre = m.Usuario?.Nombre ?? string.Empty,
                FechaMovimientoUTC = m.FechaMovimientoUTC,
                Referencia = m.Referencia,
                Observaciones = m.Observaciones,
                BodegaRelacionadaID = m.BodegaRelacionadaID,
                BodegaRelacionadaNombre = m.BodegaRelacionada?.Nombre,
                ProveedorID = m.ProveedorID,
                ProveedorNombre = m.Proveedor?.Nombre
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
                UsuarioID = m.UsuarioID,
                UsuarioNombre = m.Usuario?.Nombre ?? string.Empty,
                FechaMovimientoUTC = m.FechaMovimientoUTC,
                Referencia = m.Referencia,
                Observaciones = m.Observaciones,
                BodegaRelacionadaID = m.BodegaRelacionadaID,
                BodegaRelacionadaNombre = m.BodegaRelacionada?.Nombre,
                ProveedorID = m.ProveedorID,
                ProveedorNombre = m.Proveedor?.Nombre
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
                UsuarioID = movimiento.UsuarioID,
                UsuarioNombre = movimiento.Usuario?.Nombre ?? string.Empty,
                FechaMovimientoUTC = movimiento.FechaMovimientoUTC,
                Referencia = movimiento.Referencia,
                Observaciones = movimiento.Observaciones,
                BodegaRelacionadaID = movimiento.BodegaRelacionadaID,
                BodegaRelacionadaNombre = movimiento.BodegaRelacionada?.Nombre,
                ProveedorID = movimiento.ProveedorID,
                ProveedorNombre = movimiento.Proveedor?.Nombre
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
    }
}

