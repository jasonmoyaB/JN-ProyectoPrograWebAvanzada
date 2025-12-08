using Microsoft.EntityFrameworkCore;
using JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities;

namespace JN_ProyectoPrograAvanzadaApi_G1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<Auditoria> Auditoria { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("inv");

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles", "inv");
                entity.HasData(
                    new Rol { RolID = 1, NombreRol = "Administrador" },
                    new Rol { RolID = 2, NombreRol = "Técnico" }
                );
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios", "inv");
                entity.HasKey(e => e.UsuarioID);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CorreoElectronico).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContrasenaHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.Property(e => e.Activo).HasDefaultValue(true);
            });

            modelBuilder.Entity<Auditoria>(entity =>
            {
                entity.ToTable("Auditoria", "inv");
                entity.HasKey(e => e.AuditoriaID);
                entity.Property(e => e.Accion).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Tabla).HasMaxLength(100);
                entity.Property(e => e.IPAddress).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
            });
        }
    }
}

