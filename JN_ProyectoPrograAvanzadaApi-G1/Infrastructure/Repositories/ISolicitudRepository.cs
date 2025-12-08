using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public interface ISolicitudRepository
    {
        Task<List<Solicitud>> GetByUsuarioAsync(int usuarioId);
        Task<List<Solicitud>> GetByBodegaAsync(int bodegaId);
        Task<Solicitud?> GetByIdAsync(int solicitudId);
        Task<int> CreateAsync(Solicitud solicitud);
        Task<int> GetCountPendientesByUsuarioAsync(int usuarioId);
    }
}

