// asp_servicios/Controllers/ResenasController.cs
using Asp_servicio.Nucleo;
using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using Microsoft.AspNetCore.Mvc;

using Asp_servicio.Nucleo;

using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using Microsoft.AspNetCore.Mvc;

namespace asp_servicios.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ResenasController : ControllerBase
    {
        private IResenasAplicacion? iAplicacion = null;
        private TokenController? tokenController = null;

        public ResenasController(IResenasAplicacion? iAplicacion,
            TokenController tokenController)
        {
            this.iAplicacion = iAplicacion;
            this.tokenController = tokenController;
        }

        private Dictionary<string, object> ObtenerDatos()
        {
            var datos = new StreamReader(Request.Body).ReadToEnd().ToString();
            if (string.IsNullOrEmpty(datos))
                datos = "{}";

            return JsonConversor.ConvertirAObjeto(datos);
        }

        [HttpPost]
        public string ListarPorPropiedad()
        {
            var respuesta = new Dictionary<string, object>();
            try
            {
                var datos = ObtenerDatos();

                if (!tokenController!.Validate(datos))
                {
                    respuesta["Error"] = "lbNoAutenticacion";
                    return JsonConversor.ConvertirAString(respuesta);
                }

                int propiedadId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["PropiedadId"]));

                iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));
                var lista = iAplicacion.ListarPorPropiedad(propiedadId);

                respuesta["Entidades"] = lista;
                respuesta["Respuesta"] = "OK";
                return JsonConversor.ConvertirAString(respuesta);
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
                return JsonConversor.ConvertirAString(respuesta);
            }
        }

        [HttpPost]
        public string ObtenerPorReserva()
        {
            var respuesta = new Dictionary<string, object>();
            try
            {
                var datos = ObtenerDatos();

                if (!tokenController!.Validate(datos))
                {
                    respuesta["Error"] = "lbNoAutenticacion";
                    return JsonConversor.ConvertirAString(respuesta);
                }

                int reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"]));

                iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));
                var entidad = iAplicacion.ObtenerPorReserva(reservaId);

                respuesta["Entidad"] = entidad;
                respuesta["Respuesta"] = "OK";
                return JsonConversor.ConvertirAString(respuesta);
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
                return JsonConversor.ConvertirAString(respuesta);
            }
        }

        [HttpPost]
        public string Guardar()
        {
            var respuesta = new Dictionary<string, object>();
            try
            {
                var datos = ObtenerDatos();

                if (!tokenController!.Validate(datos))
                {
                    respuesta["Error"] = "lbNoAutenticacion";
                    return JsonConversor.ConvertirAString(respuesta);
                }

                var entidad = JsonConversor.ConvertirAObjeto<Resenas>(
                    JsonConversor.ConvertirAString(datos["Entidad"]));

                int usuarioId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["UsuarioId"]));

                iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));
                entidad = iAplicacion.Guardar(entidad, usuarioId);

                respuesta["Entidad"] = entidad;
                respuesta["Respuesta"] = "OK";
                return JsonConversor.ConvertirAString(respuesta);
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
                return JsonConversor.ConvertirAString(respuesta);
            }
        }
    }
}
