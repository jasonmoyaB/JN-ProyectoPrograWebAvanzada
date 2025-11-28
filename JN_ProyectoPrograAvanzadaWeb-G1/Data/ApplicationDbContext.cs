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
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<EntradaMercancia> EntradasMercancia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("inv");

            // ==========================
            // CONFIGURACIÓN DE EntradaMercancia
            // ==========================
            modelBuilder.Entity<EntradaMercancia>(entity =>
            {
                entity.ToTable("EntradasMercancia", "inv");

                // PRIMARY KEY
                entity.HasKey(e => e.EntradaID);

                // DEFAULT DE FECHA
                entity.Property(e => e.FechaEntrada)
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                // CANTIDAD DECIMAL CORRECTO
                entity.Property(e => e.Cantidad)
                      .HasColumnType("decimal(18,2)");

                // TipoMovimiento
                entity.Property(e => e.TipoMovimiento)
                      .HasMaxLength(20)
                      .IsRequired();

                // FK USUARIO
                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioID)
                      .OnDelete(DeleteBehavior.Restrict);

                // FK PRODUCTO
                entity.HasOne(e => e.Producto)
                      .WithMany()
                      .HasForeignKey(e => e.ProductoID)
                      .OnDelete(DeleteBehavior.Restrict);

                // FK BODEGA
                entity.HasOne(e => e.Bodega)
                      .WithMany()
                      .HasForeignKey(e => e.BodegaID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================
            // Tabla Roles
            // ==========================
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles", "inv");
                entity.HasData(
                    new Rol { RolID = 1, NombreRol = "Administrador" },
                    new Rol { RolID = 2, NombreRol = "Técnico" }
                );
            });

            // ==========================
            // Config usuario
            // ==========================
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

            // ==========================
            // Tabla Auditoría
            // ==========================
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
