using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using lib_presentaciones.Interfaces;

namespace lib_presentaciones.Implementaciones
{
    public class BusquedasPresentacion : IBusquedasPresentacion
    {
        private Comunicaciones? comunicaciones = null;

       

        public async Task<List<Propiedades>> Filtro(Busquedas? entidad)
        {
            var lista = new List<Propiedades>();
            var datos = new Dictionary<string, object>();
            datos["Entidad"] = entidad!;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Busquedas/Filtro");
            var respuesta = await comunicaciones!.Execute(datos);

            if (respuesta.ContainsKey("Error"))
            {
                throw new Exception(respuesta["Error"].ToString()!);
            }
            lista = JsonConversor.ConvertirAObjeto<List<Propiedades>>(
                JsonConversor.ConvertirAString(respuesta["Entidades"]));
            return lista;
        }

    

       

       
    }
}