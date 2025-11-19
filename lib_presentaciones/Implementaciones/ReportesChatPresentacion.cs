using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using lib_presentaciones.Interfaces;

namespace lib_presentaciones.Implementaciones
{
    public class ReportesChatPresentacion : IReportesChatPresentacion
    {
        private Comunicaciones? comunicaciones;

        public async Task<ReporteChat> Crear(int reservaId, int usuarioReportanteId, string motivo)
        {
            var datos = new Dictionary<string, object>
            {
                ["ReservaId"] = reservaId,
                ["UsuarioReportanteId"] = usuarioReportanteId,
                ["Motivo"] = motivo
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "ReportesChat/Crear");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            return JsonConversor.ConvertirAObjeto<ReporteChat>(
                JsonConversor.ConvertirAString(respuesta["Entidad"])
            );
        }

        public async Task<ReporteChat?> ObtenerActivoPorReserva(int reservaId)
        {
            var datos = new Dictionary<string, object>
            {
                ["ReservaId"] = reservaId
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "ReportesChat/ObtenerActivoPorReserva");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            // puede venir null
            var json = JsonConversor.ConvertirAString(respuesta["Entidad"]);
            if (string.IsNullOrWhiteSpace(json) || json == "null")
                return null;

            return JsonConversor.ConvertirAObjeto<ReporteChat>(json);
        }

        public async Task<List<ReporteChat>> ListarActivos()
        {
            var datos = new Dictionary<string, object>();

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "ReportesChat/ListarActivos");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            return JsonConversor.ConvertirAObjeto<List<ReporteChat>>(
                JsonConversor.ConvertirAString(respuesta["Entidades"])
            );
        }

        public async Task<ReporteChat?> ObtenerPorId(int id)
        {
            var datos = new Dictionary<string, object>
            {
                ["Id"] = id
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "ReportesChat/ObtenerPorId");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            var json = JsonConversor.ConvertirAString(respuesta["Entidad"]);
            if (string.IsNullOrWhiteSpace(json) || json == "null")
                return null;

            return JsonConversor.ConvertirAObjeto<ReporteChat>(json);
        }

        // 🔴 AQUÍ se alinea con tu controlador: se envía "Id"
        public async Task Finalizar(int id)
        {
            var datos = new Dictionary<string, object>
            {
                ["Id"] = id
            };

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "ReportesChat/Finalizar");
            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"]!.ToString());

            // No hace falta devolver nada, con que no lance error es suficiente
        }
    }
}