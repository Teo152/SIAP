using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces
{
    public interface IMunicipiosPresentacion
    {
        Task<List<Municipios>> Listar();
       
        
    }
}

