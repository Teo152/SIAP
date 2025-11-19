using Asp_servicio.Nucleo;
using lib_aplicaciones.Interfaces;
using lib_dominio.Nucleo;
using Microsoft.AspNetCore.Mvc;

namespace asp_servicios.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportesChatController : ControllerBase
    {
        private readonly IReportesChatAplicacion? _app;
        private readonly TokenController? _tokenController;

        public ReportesChatController(
            IReportesChatAplicacion? app,
            TokenController tokenController)
        {
            _app = app;
            _tokenController = tokenController;
        }

        private Dictionary<string, object> ObtenerDatos()
        {
            var datos = new StreamReader(Request.Body).ReadToEnd() ?? "{}";
            if (string.IsNullOrWhiteSpace(datos))
                datos = "{}";

            return JsonConversor.ConvertirAObjeto(datos);
        }

        // POST: /ReportesChat/Crear
        [HttpPost]
        public string Crear()
        {
            var respuesta = new Dictionary<string, object>();

            try
            {
                var datos = ObtenerDatos();

                if (!_tokenController!.Validate(datos))
                {
                    respuesta["Error"] = "lbNoAutenticacion";
                    return JsonConversor.ConvertirAString(respuesta);
                }

                _app!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"]));

                var usuarioReportanteId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["UsuarioReportanteId"]));

                var motivo = datos.ContainsKey("Motivo")
                    ? JsonConversor.ConvertirAString(datos["Motivo"])
                    : "";

                var reporte = _app.Crear(reservaId, usuarioReportanteId, motivo);

                respuesta["Entidad"] = reporte;
                respuesta["Respuesta"] = "OK";
                respuesta["Fecha"] = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }

        // POST: /ReportesChat/ObtenerActivoPorReserva
        [HttpPost]
        public string ObtenerActivoPorReserva()
        {
            var respuesta = new Dictionary<string, object>();

            try
            {
                var datos = ObtenerDatos();

                if (!_tokenController!.Validate(datos))
                {
                    respuesta["Error"] = "lbNoAutenticacion";
                    return JsonConversor.ConvertirAString(respuesta);
                }

                _app!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"]));

                var reporte = _app.ObtenerActivoPorReserva(reservaId);

                respuesta["Entidad"] = reporte;
                respuesta["Respuesta"] = "OK";
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }

        // POST: /ReportesChat/ListarActivos
        [HttpPost]
        public string ListarActivos()
        {
            var respuesta = new Dictionary<string, object>();

            try
            {
                var datos = ObtenerDatos();

                if (!_tokenController!.Validate(datos))
                {
                    respuesta["Error"] = "lbNoAutenticacion";
                    return JsonConversor.ConvertirAString(respuesta);
                }

                _app!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                var lista = _app.ListarActivos();

                respuesta["Entidades"] = lista;
                respuesta["Respuesta"] = "OK";
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }

        // POST: /ReportesChat/ObtenerPorId
        [HttpPost]
        public string ObtenerPorId()
        {
            var respuesta = new Dictionary<string, object>();

            try
            {
                var datos = ObtenerDatos();

                if (!_tokenController!.Validate(datos))
                {
                    respuesta["Error"] = "lbNoAutenticacion";
                    return JsonConversor.ConvertirAString(respuesta);
                }

                _app!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                var id = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["Id"]));

                var reporte = _app.ObtenerPorId(id);

                respuesta["Entidad"] = reporte;
                respuesta["Respuesta"] = "OK";
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }

        [HttpPost]
        public string Finalizar()
        {
            var respuesta = new Dictionary<string, object>();

            try
            {
                var datos = ObtenerDatos();

                if (!_tokenController!.Validate(datos))
                {
                    respuesta["Error"] = "lbNoAutenticacion";
                    return JsonConversor.ConvertirAString(respuesta);
                }

                this._app!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                if (!datos.ContainsKey("Id"))
                    throw new Exception("lbFaltaId");

                var id = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["Id"])
                );

                this._app!.Finalizar(id);

                respuesta["Respuesta"] = "OK";
                respuesta["Fecha"] = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }
    }
}