using Microsoft.EntityFrameworkCore;
using JN_ProyectoPrograAvanzadaWeb_G1.Models;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

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

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("Auditoria", "inv");  
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username).HasMaxLength(100);
                entity.Property(e => e.ActionType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityId).HasMaxLength(100);
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.Property(e => e.Source).HasMaxLength(50);

                entity.Property(e => e.BeforeState).HasColumnType("nvarchar(max)");
                entity.Property(e => e.AfterState).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Meta).HasColumnType("nvarchar(max)");

                entity.HasIndex(e => e.OccurredAt);
                entity.HasIndex(e => e.Username);
                entity.HasIndex(e => new { e.EntityName, e.EntityId });
            });

        }
    }
}
