using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_repositorios.Implementaciones
{
    public partial class Conexion : DbContext, IConexion
    {
        public string? StringConexion { get; set; }

        public Conexion(DbContextOptions<Conexion> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(StringConexion))
            {
                optionsBuilder.UseSqlServer(StringConexion);
            }

            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<Busquedas>? Busquedas { get; set; }
        public DbSet<Mensajes>? Mensajes { get; set; }
        public DbSet<Propiedades>? Propiedades { get; set; }
        public DbSet<Pagos>? Pagos { get; set; }
        public DbSet<Resenas>? Resenas { get; set; }
        public DbSet<Reservas>? Reservas { get; set; }
        public DbSet<Usuarios>? Usuarios { get; set; }
        public DbSet<Municipios>? Municipios { get; set; }
        public DbSet<ReporteChat> ReportesChat { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Mensajes>()
                .HasOne(m => m.Remitente)
                .WithMany()
                .HasForeignKey(m => m.RemitenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mensajes>()
                .HasOne(m => m.Destinatario)
                .WithMany()
                .HasForeignKey(m => m.DestinatarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pagos>()
          .HasOne(m => m.Reserva
          )
          .WithMany()
          .HasForeignKey(m => m.ReservaId)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pagos>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReporteChat>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.Reserva)
                      .WithMany()
                      .HasForeignKey(r => r.ReservaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.UsuarioReportante)
                      .WithMany()
                      .HasForeignKey(r => r.UsuarioReportanteId)
                      .OnDelete(DeleteBehavior.NoAction);

                base.OnModelCreating(modelBuilder);

            });

            modelBuilder.Entity<Resenas>(entity =>
            {
                entity.ToTable("Resenas");

                entity.Property(r => r.Comentario)
                      .HasMaxLength(500);

                entity.Property(r => r.Calificacion)
                      .IsRequired();

                entity.HasOne(r => r.Propiedad)
                      .WithMany(p => p.Resenas)
                      .HasForeignKey(r => r.PropiedadId);

            //    entity.HasOne(r => r.Reserva)
            //         .WithOne(rv => rv.Resenas)   // si solo permites 1 reseña por reserva
            //          .HasForeignKey<Resenas>(r => r.ReservaId);
            });


        }
    }
}