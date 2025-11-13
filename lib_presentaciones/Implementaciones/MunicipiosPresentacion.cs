using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using lib_presentaciones.Interfaces;

namespace lib_presentaciones.Implementaciones
{
    public class MunicipiosPresentacion : IMunicipiosPresentacion
    {
        private Comunicaciones? comunicaciones = null;

        public async Task<List<Municipios>> Listar()
        {
            var lista = new List<Municipios>();
            var datos = new Dictionary<string, object>();

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Municipios/Listar");
            var respuesta = await comunicaciones!.Execute(datos);

            if (respuesta.ContainsKey("Error"))
            {
                throw new Exception(respuesta["Error"].ToString()!);
            }
            lista = JsonConversor.ConvertirAObjeto<List<Municipios>>(
                JsonConversor.ConvertirAString(respuesta["Entidades"]));

            return lista;
        }
    }
}