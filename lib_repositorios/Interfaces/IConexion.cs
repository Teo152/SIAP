using lib_dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace lib_repositorios.Interfaces
{
    public interface IConexion
    {
        string? StringConexion { get; set; }

        DbSet<Busquedas>? Busquedas { get; set; }

        DbSet<Estados>? Estados { get; set; }
        DbSet<Mensajes>? Mensajes { get; set; }

        DbSet<Propiedades>? Propiedades { get; set; }

        DbSet<Pagos>? Pagos { get; set; }
        DbSet<Resenas>? Resenas { get; set; }
        DbSet<Reservas>? Reservas { get; set; }
        DbSet<Usuarios>? Usuarios { get; set; }



        EntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
    }
}