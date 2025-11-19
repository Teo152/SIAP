// lib_presentaciones/Implementaciones/ResenasPresentacion.cs
using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using lib_presentaciones.Interfaces;

namespace lib_presentaciones.Implementaciones
{
    public class ResenasPresentacion : IResenasPresentacion
    {
        private Comunicaciones? comunicaciones = null;

        public async Task<List<Resenas>> ListarPorPropiedad(int propiedadId)
        {
            var datos = new Dictionary<string, object>();
            datos["PropiedadId"] = propiedadId;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Resenas/ListarPorPropiedad");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"].ToString()!);

            return JsonConversor.ConvertirAObjeto<List<Resenas>>(
                JsonConversor.ConvertirAString(respuesta["Entidades"]));
        }

        public async Task<Resenas?> ObtenerPorReserva(int reservaId)
        {
            var datos = new Dictionary<string, object>();
            datos["ReservaId"] = reservaId;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Resenas/ObtenerPorReserva");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"].ToString()!);

            if (!respuesta.ContainsKey("Entidad") || respuesta["Entidad"] == null)
                return null;

            return JsonConversor.ConvertirAObjeto<Resenas>(
                JsonConversor.ConvertirAString(respuesta["Entidad"]));
        }

        public async Task<Resenas?> Guardar(Resenas entidad, int usuarioId)
        {
            var datos = new Dictionary<string, object>();
            datos["Entidad"] = entidad;
            datos["UsuarioId"] = usuarioId;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Resenas/Guardar");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"].ToString()!);

            return JsonConversor.ConvertirAObjeto<Resenas>(
                JsonConversor.ConvertirAString(respuesta["Entidad"]));
        }
    }
}
