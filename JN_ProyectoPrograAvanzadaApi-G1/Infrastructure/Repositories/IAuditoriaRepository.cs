using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public interface IAuditoriaRepository
    {
        Task<int> RegistrarAsync(Auditoria auditoria);
        Task<List<Auditoria>> GetByUsuarioAsync(int usuarioId, DateTime? desde = null, DateTime? hasta = null);
        Task<List<Auditoria>> GetByAccionAsync(string accion, DateTime? desde = null, DateTime? hasta = null);
        Task<List<Auditoria>> GetAllAsync(DateTime? desde = null, DateTime? hasta = null, int? limit = null);
    }
}

