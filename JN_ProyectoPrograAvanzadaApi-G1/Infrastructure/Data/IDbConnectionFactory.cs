

using System.Data;

namespace JN_ProyectoPrograAvanzadaApi_G1.Infrastructure.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}

