using Dapper;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RolRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Rol>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var roles = await connection.QueryAsync<Rol>(
                "inv.sp_Rol_GetAll",
                commandType: CommandType.StoredProcedure);

            return roles.ToList();
        }

        public async Task<Rol?> GetByIdAsync(int rolId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Rol>(
                "inv.sp_Rol_GetById",
                new { RolID = rolId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Rol?> GetByNombreAsync(string nombreRol)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Rol>(
                "inv.sp_Rol_GetByNombre",
                new { NombreRol = nombreRol },
                commandType: CommandType.StoredProcedure);
        }
    }
}

