using lib_dominio.Entidades;

namespace lib_aplicaciones.Interfaces
{
    public interface IMunicipiosAplicacion
    {
        void Configurar(string StringConexion);

        List<Municipios> Listar();

    }
}