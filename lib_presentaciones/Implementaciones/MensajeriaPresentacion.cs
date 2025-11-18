using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using lib_presentaciones.Interfaces;

namespace lib_presentaciones.Implementaciones
{
    public class MensajeriaPresentacion : IMensajeriaPresentacion
    {
        private Comunicaciones? comunicaciones = null;

        public async Task<Mensajes> Enviar(int reservaId, Mensajes entidad)
        {
            var datos = new Dictionary<string, object>();
            var mensaje = new Mensajes();

            datos["ReservaId"] = reservaId;
            datos["Entidad"] = entidad;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Mensajeria/Enviar");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
            {
                throw new Exception(respuesta["Error"]!.ToString());
            }

            mensaje = JsonConversor.ConvertirAObjeto<Mensajes>(
                JsonConversor.ConvertirAString(respuesta["Entidad"]));

            return mensaje;
        }

        public async Task<List<Mensajes>> ListarConversacion(int usuario1Id, int usuario2Id)
        {
            var lista = new List<Mensajes>();
            var datos = new Dictionary<string, object>();

            datos["Usuario1Id"] = usuario1Id;
            datos["Usuario2Id"] = usuario2Id;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Mensajeria/ListarConversacion");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
            {
                throw new Exception(respuesta["Error"]!.ToString());
            }

            lista = JsonConversor.ConvertirAObjeto<List<Mensajes>>(
                JsonConversor.ConvertirAString(respuesta["Entidades"]));

            return lista;
        }

        public async Task<int> ContarNoLeidos(int usuarioDestinoId, int otroUsuarioId)
        {
            var datos = new Dictionary<string, object>
            {
                ["UsuarioDestinoId"] = usuarioDestinoId,
                ["OtroUsuarioId"] = otroUsuarioId
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Mensajeria/ContarNoLeidos");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            return JsonConversor.ConvertirAObjeto<int>(
                JsonConversor.ConvertirAString(respuesta["Respuesta"])
            );
        }

        public async Task MarcarComoLeidos(int usuarioDestinoId, int otroUsuarioId)
        {
            var datos = new Dictionary<string, object>
            {
                ["UsuarioDestinoId"] = usuarioDestinoId,
                ["OtroUsuarioId"] = otroUsuarioId
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Mensajeria/MarcarComoLeidos");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());
        }

        public async Task<bool> AdminPuedeIngresar(int reservaId)
        {
            var datos = new Dictionary<string, object>
            {
                ["ReservaId"] = reservaId
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Mensajeria/AdminPuedeIngresar");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            // La API devuelve {"PuedeIngresar": true/false, "Respuesta": "OK"}
            var puede = JsonConversor.ConvertirAObjeto<bool>(
                JsonConversor.ConvertirAString(respuesta["PuedeIngresar"])
            );

            return puede;
        }

        public async Task<List<Mensajes>> AdminListarConversacion(int reservaId)
        {
            var datos = new Dictionary<string, object>
            {
                ["ReservaId"] = reservaId
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Mensajeria/AdminListarConversacion");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            var lista = JsonConversor.ConvertirAObjeto<List<Mensajes>>(
                JsonConversor.ConvertirAString(respuesta["Entidades"])
            );

            return lista;
        }

        public async Task<Mensajes> AdminEnviar(int reservaId, int adminId, string texto)
        {
            var datos = new Dictionary<string, object>
            {
                ["ReservaId"] = reservaId,
                ["AdminId"] = adminId,
                ["Texto"] = texto
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Mensajeria/AdminEnviar");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            var mensaje = JsonConversor.ConvertirAObjeto<Mensajes>(
                JsonConversor.ConvertirAString(respuesta["Entidad"])
            );

            return mensaje;
        }
    }
}