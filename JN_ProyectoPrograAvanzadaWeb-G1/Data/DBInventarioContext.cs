using Microsoft.EntityFrameworkCore;


namespace JN_ProyectoPrograAvanzadaWeb_G1.Data
{
    public class DBInventarioContext : DbContext
    {
        public DBInventarioContext(DbContextOptions<DBInventarioContext> options)
            : base(options)
        {
        }

    }
}
