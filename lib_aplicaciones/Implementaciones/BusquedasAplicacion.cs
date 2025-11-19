
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

            // 1️⃣ FILTRO POR CIUDAD
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

            // 2️⃣ FILTRO POR CAPACIDAD
            if (entidad.Cantidad_Huespedes.HasValue)
            {
                query = query.Where(p => p.Capacidad >= entidad.Cantidad_Huespedes.Value);
            }

            // 3️⃣ DISPONIBILIDAD POR FECHAS (hecha 100% en SQL)
            // 3️⃣ DISPONIBILIDAD POR FECHAS (hecha 100% en SQL)
            if (entidad.Fecha_deseada.HasValue && entidad.Fecha_Fin.HasValue)
            {
                var inicio = entidad.Fecha_deseada.Value.Date;
                var fin = entidad.Fecha_Fin.Value.Date;

                if (fin <= inicio)
                    return new List<Propiedades>();

                query = query.Where(p =>
                    !this.IConexion!.Reservas!.Any(r =>
                        r.PropiedadId == p.Id &&

                        // 👇 AQUÍ ES LA CLAVE:
                        // Solo NO bloquean las reservas canceladas o completadas.
                        // Ajusta los valores 4 y 5 a los que uses en tu enum.
                        r.EstadoId != 4 &&   // por ejemplo: 4 = Cancelada
                        r.EstadoId != 5 &&   // por ejemplo: 5 = Completada

                        // cruce de rangos de fechas
                        inicio < r.Fecha_fin &&
                        fin > r.Fecha_deseada
                    )
                );
            }


            return query.ToList();
        }




    }
}