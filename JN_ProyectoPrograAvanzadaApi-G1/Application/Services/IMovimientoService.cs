using JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Movimientos;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.Services
{
    public interface IMovimientoService
    {
        Task<List<MovimientoDto>> GetByBodegaAsync(int bodegaId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<List<MovimientoDto>> GetByUsuarioAsync(int usuarioId, DateTime? fechaDesde = null, DateTime? fechaHasta = null);
        Task<MovimientoDto?> GetByIdAsync(int movimientoId);
        Task<int> GetCountMovimientosHoyByBodegaAsync(int bodegaId);
        Task<int> GetCountMovimientosHoyByUsuarioAsync(int usuarioId);
        Task<int> CreateTrasladoAsync(MovimientoTrasladoDto dto, int usuarioId);
    }
}

