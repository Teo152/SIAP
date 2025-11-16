using lib_dominio.Entidades;

namespace lib_aplicaciones.Interfaces
{
    public interface IPropiedadesAplicacion
    {
        void Configurar(string StringConexion);
        List<Propiedades> PorNombre(Propiedades? entidad);
        List<Propiedades> Listar();
        Propiedades? Guardar(Propiedades? entidad);
        Propiedades? Modificar(Propiedades? entidad);
        Propiedades? Borrar(Propiedades? entidad);
    
    }
}
