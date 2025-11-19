using System.Data;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Infrastructure.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}

