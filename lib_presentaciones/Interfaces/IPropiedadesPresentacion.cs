using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces
{
    public interface IPropiedadesPresentacion
    {
        Task<List<Propiedades>> Listar();
        Task<List<Propiedades>> PorNombre(Propiedades? entidad);
        Task<Propiedades?> Guardar(Propiedades? entidad);
        Task<Propiedades?> Modificar(Propiedades? entidad);
        Task<Propiedades?> Borrar(Propiedades? entidad);
       

    }
}

