using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public interface IMovimientoRepository
    {
        Task<List<Movimiento>> GetByBodegaAsync(int bodegaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<List<Movimiento>> GetByUsuarioAsync(int usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<Movimiento?> GetByIdAsync(int movimientoId);
        Task<int> GetCountMovimientosHoyByBodegaAsync(int bodegaId);
        Task<int> GetCountMovimientosHoyByUsuarioAsync(int usuarioId);
        Task<int> CreateAsync(Movimiento movimiento, IDbTransaction? transaction = null);
    }
}

