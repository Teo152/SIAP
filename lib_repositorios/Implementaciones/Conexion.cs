using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_repositorios.Implementaciones
{
    public partial class Conexion : DbContext, IConexion
    {
        public string? StringConexion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        {
            optionsBuilder.UseSqlServer(this.StringConexion!, p => { });
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        }

        public DbSet<Busquedas>? Busquedas { get; set; }

      //  public DbSet<Estados>? Estados { get; set; }  Presguntar por este
       public DbSet<Mensajes>? Mensajes { get; set; }

        public DbSet<Propiedades>? Propiedades { get; set; }

        public DbSet<Pagos>? Pagos { get; set; }
        public DbSet<Resenas>? Resenas { get; set; }
        public DbSet<Reservas>? Reservas { get; set; }
        public DbSet<Usuarios>? Usuarios { get; set; }

    }
}