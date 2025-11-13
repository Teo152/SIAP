using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces
{
    public interface IBusquedasPresentacion
    {

        Task<List<Propiedades>> Filtro(Busquedas? entidad);

    }
}