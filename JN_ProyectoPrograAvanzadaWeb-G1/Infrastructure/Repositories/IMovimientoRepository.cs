using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public interface IMovimientoRepository
    {
        Task<List<Movimiento>> GetByBodegaAsync(int bodegaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<List<Movimiento>> GetByUsuarioAsync(int usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<Movimiento?> GetByIdAsync(int movimientoId);
        Task<int> GetCountMovimientosHoyByBodegaAsync(int bodegaId);
        Task<int> GetCountMovimientosHoyByUsuarioAsync(int usuarioId);
    }
}

