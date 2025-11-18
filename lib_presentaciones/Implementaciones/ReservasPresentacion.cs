using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using lib_presentaciones.Interfaces;

namespace lib_presentaciones.Implementaciones
{
    public class ReservasPresentacion : IReservasPresentacion
    {
        private Comunicaciones? comunicaciones = null;

        public async Task<List<Reservas>> Listar()
        {
            var lista = new List<Reservas>();
            var datos = new Dictionary<string, object>();

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Reservas/Listar");
            var respuesta = await comunicaciones!.Execute(datos);

            if (respuesta.ContainsKey("Error"))
            {
                throw new Exception(respuesta["Error"].ToString()!);
            }
            lista = JsonConversor.ConvertirAObjeto<List<Reservas>>(
                JsonConversor.ConvertirAString(respuesta["Entidades"]));

            return lista;
        }

        public async Task<Reservas?> Guardar(Reservas? entidad)
        {
            if (entidad!.Id != 0)
            {
                throw new Exception("lbFaltaInformacion");
            }

            var datos = new Dictionary<string, object>();
            datos["Entidad"] = entidad;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Reservas/Guardar");
            var respuesta = await comunicaciones!.Execute(datos);

            if (respuesta.ContainsKey("Error"))
            {
                throw new Exception(respuesta["Error"].ToString()!);
            }
            entidad = JsonConversor.ConvertirAObjeto<Reservas>(
                JsonConversor.ConvertirAString(respuesta["Entidad"]));
            return entidad;
        }

        public async Task<List<Reservas>> ListarParaMensajeria(int usuarioId)
        {
            var datos = new Dictionary<string, object>();

            datos["Entidad"] = new Reservas
            {
                UsuarioId = usuarioId
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Reservas/ListarParaMensajeria");

            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
            {
                throw new Exception(respuesta["Error"]!.ToString());
            }

            if (!respuesta.ContainsKey("Entidades") || respuesta["Entidades"] == null)
            {
                return new List<Reservas>();
            }

            var reservas = JsonConversor.ConvertirAObjeto<List<Reservas>>(
                JsonConversor.ConvertirAString(respuesta["Entidades"])
            );

            return reservas;
        }

        public async Task<Reservas?> PorId(int reservaId)
        {
            var datos = new Dictionary<string, object>
            {
                ["ReservaId"] = reservaId
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Reservas/PorId");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            return JsonConversor.ConvertirAObjeto<Reservas?>(
                JsonConversor.ConvertirAString(respuesta["Entidad"])
            );
        }
    }
}