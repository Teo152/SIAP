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

        public List<Propiedades> Filtro(Busquedas? entidad)
        {
            if (entidad == null)
                throw new Exception("Debe especificar al menos un criterio de búsqueda.");

            var query = this.IConexion!.Propiedades!.AsQueryable();

            if (!string.IsNullOrEmpty(entidad.Ciudad))
            {
                var municipio = this.IConexion!.Municipios!
                    .FirstOrDefault(m => m.Nombre == entidad.Ciudad);

                if (municipio != null)
                {
                    query = query.Where(p => p.MunicipioId == municipio.Id);
                }
                else
                {
                    return new List<Propiedades>();
                }
            }

            if (entidad.Cantidad_Huespedes.HasValue)
            {
                query = query.Where(p => p.Capacidad >= entidad.Cantidad_Huespedes.Value);
            }

            // 3️⃣ FUTURO: FILTRO POR FECHAS (CUANDO IMPLEMENTES RESERVAS)
            // if (entidad.CheckIn.HasValue && entidad.CheckOut.HasValue)
            // {
            //     var checkIn = entidad.CheckIn.Value;
            //     var checkOut = entidad.CheckOut.Value;
            //
            //     query = query.Where(p =>
            //         !p.Reservas.Any(r =>
            //             (checkIn < r.FechaFin && checkOut > r.FechaInicio)
            //         ));
            // }

            return query.ToList();
        }
    }
}