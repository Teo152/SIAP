using Asp_servicio.Nucleo;

using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using Microsoft.AspNetCore.Mvc;

namespace asp_servicios.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class PagosController : ControllerBase
    {
        private IPagosAplicacion? iAplicacion = null;
        private TokenController? tokenController = null;

        public PagosController(IPagosAplicacion? iAplicacion,
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
        public string Procesar_pago()
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

                var entidad = JsonConversor.ConvertirAObjeto<Pagos>(
                    JsonConversor.ConvertirAString(datos["Entidad"]));

                this.iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));
                var mensaje = this.iAplicacion!.Procesar_pago(entidad);


                respuesta["Mensaje"] = mensaje;

                respuesta["Respuesta"] = "OK";
                respuesta["Fecha"] = DateTime.Now.ToString();
                return JsonConversor.ConvertirAString(respuesta);
            }
            catch (Exception ex)
            {
                Exception? inner = ex;
                while (inner?.InnerException != null)
                    inner = inner.InnerException;

                // Limpiamos el mensaje
                string errorMensaje = inner?.Message ?? "Error desconocido en el servidor.";
                errorMensaje = errorMensaje.Replace("\"", "'").Replace("\r", " ").Replace("\n", " ").Trim();

                // Devolvemos un JSON manualmente formateado (no volvemos a serializar)
                var json = $"{{\"Error\":\"{errorMensaje}\"}}";

                Console.WriteLine("⚠️ ERROR EN BACKEND: " + errorMensaje);
                return json;
            }


        }

        

       
    }
}