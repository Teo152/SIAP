
using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_aplicaciones.Implementaciones
{
    public class BusquedasAplicacion : IBusquedasAplicacion
    {
        private IConexion? IConexion = null;

        public BusquedasAplicacion(IConexion iConexion)


        {
            this.IConexion = iConexion;
        }

        public void Configurar(string StringConexion)
        {
            this.IConexion!.StringConexion = StringConexion;
        }

        //public List<Propiedades> BuscarPropiedades(Busquedas filtros)
        //{
        //    var query = this.IConexion.Propiedades;

        //    if (!string.IsNullOrEmpty(filtros.ciudad))
        //        query = query.Where(p => p.Direccion.Contains(filtros.Ciudad));

        //    if (!string.IsNullOrEmpty(filtros.TipoPropiedad))
        //        query = query.Where(p => p.TipoPropiedad == filtros.TipoPropiedad);

        //    if (filtros.PrecioMin.HasValue)
        //        query = query.Where(p => p.Precio >= filtros.PrecioMin.Value);

        //    if (filtros.PrecioMax.HasValue)
        //        query = query.Where(p => p.Precio <= filtros.PrecioMax.Value);

        //    return query.ToList();
        //}
    }
}