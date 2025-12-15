using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public interface ISolicitudRepository
    {
        Task<List<Solicitud>> GetAllAsync();
        Task<List<Solicitud>> GetByUsuarioAsync(int usuarioId);
        Task<List<Solicitud>> GetByBodegaAsync(int bodegaId);
        Task<Solicitud?> GetByIdAsync(int solicitudId);
        Task<int> CreateAsync(Solicitud solicitud);
        Task<int> GetCountPendientesByUsuarioAsync(int usuarioId);
        Task<bool> UpdateEstadoAsync(int solicitudId, int estadoSolicitudId, int? usuarioAprobadorId = null, DateTime? fechaAprobacion = null, DateTime? fechaEnvio = null, DateTime? fechaEntrega = null, string? comentarios = null);
        Task<bool> EstablecerCantidadesEnviadasAsync(int solicitudId, Dictionary<int, decimal> cantidades);
        Task<bool> TieneCantidadesEnviadasAsync(int solicitudId);
    }
}

