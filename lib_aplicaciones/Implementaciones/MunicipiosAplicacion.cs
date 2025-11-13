using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_aplicaciones.Implementaciones
{
    public class MunicipiosAplicacion : IMunicipiosAplicacion
    {
        private IConexion? IConexion = null;

        public MunicipiosAplicacion(IConexion iConexion)


        {
            this.IConexion = iConexion;
        }

        public void Configurar(string StringConexion)
        {
            this.IConexion!.StringConexion = StringConexion;
        }
        public List<Municipios> Listar()
        {
            return this.IConexion!.Municipios!.Take(20).ToList();
        }
    }
}