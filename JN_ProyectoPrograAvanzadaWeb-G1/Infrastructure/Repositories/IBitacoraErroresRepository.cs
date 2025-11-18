using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public interface IBitacoraErroresRepository
    {
        Task<int> RegistrarAsync(BitacoraErrores error);
        Task<List<BitacoraErrores>> GetByUsuarioAsync(int? usuarioId, DateTime? desde = null, DateTime? hasta = null);
        Task<List<BitacoraErrores>> GetAllAsync(DateTime? desde = null, DateTime? hasta = null, int? limit = 100);
    }
}

