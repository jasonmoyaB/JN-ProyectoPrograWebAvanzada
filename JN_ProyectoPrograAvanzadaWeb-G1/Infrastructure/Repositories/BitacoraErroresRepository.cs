using Dapper;
using JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities;
using JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Data;
using System.Data;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Repositories
{
    public class BitacoraErroresRepository : IBitacoraErroresRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BitacoraErroresRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> RegistrarAsync(BitacoraErrores error)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UsuarioID", error.UsuarioID);
                parameters.Add("@Error", error.Error);
                parameters.Add("@StackTrace", error.StackTrace);
                parameters.Add("@Controlador", error.Controlador);
                parameters.Add("@Accion", error.Accion);
                parameters.Add("@RequestPath", error.RequestPath);
                parameters.Add("@UserAgent", error.UserAgent);
                parameters.Add("@BitacoraErrorID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "inv.sp_BitacoraErrores_Registrar",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("@BitacoraErrorID");
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<BitacoraErrores>> GetByUsuarioAsync(int? usuarioId, DateTime? desde = null, DateTime? hasta = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var errores = await connection.QueryAsync<BitacoraErrores>(
                    "inv.sp_BitacoraErrores_GetByUsuario",
                    new
                    {
                        UsuarioID = usuarioId,
                        Desde = desde,
                        Hasta = hasta
                    },
                    commandType: CommandType.StoredProcedure);

                return errores.ToList();
            }
            catch
            {
                return new List<BitacoraErrores>();
            }
        }

        public async Task<List<BitacoraErrores>> GetAllAsync(DateTime? desde = null, DateTime? hasta = null, int? limit = 100)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var errores = await connection.QueryAsync<BitacoraErrores>(
                    "inv.sp_BitacoraErrores_GetAll",
                    new
                    {
                        Desde = desde,
                        Hasta = hasta,
                        Limit = limit ?? 100
                    },
                    commandType: CommandType.StoredProcedure);

                return errores.ToList();
            }
            catch
            {
                return new List<BitacoraErrores>();
            }
        }
    }
}

