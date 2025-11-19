using Asp_servicio.Nucleo;
using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using Microsoft.AspNetCore.Mvc;

namespace asp_servicios.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MensajeriaController : ControllerBase
    {
        private IMensajeriaAplicacion? iAplicacion = null;
        private TokenController? tokenController = null;

        public MensajeriaController(IMensajeriaAplicacion? iAplicacion,
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
        public string Enviar()
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

                this.iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                if (!datos.ContainsKey("ReservaId"))
                    throw new Exception("lbFaltaReservaId");

                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"])
                );

                if (!datos.ContainsKey("Entidad"))
                    throw new Exception("lbFaltaEntidad");

                var entidad = JsonConversor.ConvertirAObjeto<Mensajes>(
                    JsonConversor.ConvertirAString(datos["Entidad"])
                );

                var mensajeGuardado = this.iAplicacion!.Enviar(entidad, reservaId);

                respuesta["Entidad"] = mensajeGuardado;
                respuesta["Respuesta"] = "OK";
                respuesta["Fecha"] = DateTime.Now.ToString();
                return JsonConversor.ConvertirAString(respuesta);
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message.ToString();
                return JsonConversor.ConvertirAString(respuesta);
            }
        }

        [HttpPost]
        public string ListarConversacion()
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

                this.iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                if (!datos.ContainsKey("Usuario1Id"))
                    throw new Exception("lbFaltaUsuario1Id");

                if (!datos.ContainsKey("Usuario2Id"))
                    throw new Exception("lbFaltaUsuario2Id");

                if (!datos.ContainsKey("ReservaId"))
                    throw new Exception("lbFaltaReservaId");

                var usuario1Id = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["Usuario1Id"])
                );

                var usuario2Id = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["Usuario2Id"])
                );
                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"])
                );

                var lista = this.iAplicacion!.ListarConversacion(usuario1Id, usuario2Id, reservaId);

                respuesta["Entidades"] = lista;
                respuesta["Respuesta"] = "OK";
                respuesta["Fecha"] = DateTime.Now.ToString();
                return JsonConversor.ConvertirAString(respuesta);
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message.ToString();
                return JsonConversor.ConvertirAString(respuesta);
            }
        }

        [HttpPost]
        public string ContarNoLeidos()
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

                this.iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                if (!datos.ContainsKey("UsuarioDestinoId") ||
                    !datos.ContainsKey("OtroUsuarioId") ||
                    !datos.ContainsKey("ReservaId"))
                {
                    throw new Exception("lbFaltaInformacion");
                }

                var usuarioDestinoId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["UsuarioDestinoId"])
                );

                var otroUsuarioId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["OtroUsuarioId"])
                );

                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"])
                );

                var cantidad = this.iAplicacion!.ContarNoLeidos(
                    usuarioDestinoId,
                    otroUsuarioId,
                    reservaId
                );

                respuesta["Respuesta"] = cantidad;
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }

        [HttpPost]
        public string MarcarComoLeidos()
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

                this.iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                if (!datos.ContainsKey("UsuarioDestinoId") ||
                    !datos.ContainsKey("OtroUsuarioId") ||
                    !datos.ContainsKey("ReservaId"))
                {
                    throw new Exception("lbFaltaInformacion");
                }

                var usuarioDestinoId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["UsuarioDestinoId"])
                );

                var otroUsuarioId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["OtroUsuarioId"])
                );

                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"])
                );

                this.iAplicacion!.MarcarComoLeidos(
                    usuarioDestinoId,
                    otroUsuarioId,
                    reservaId
                );

                respuesta["Respuesta"] = "OK";
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }

        [HttpPost]
        public string AdminPuedeIngresar()
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

                iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"])
                );

                var puede = iAplicacion.AdminPuedeIngresar(reservaId);

                respuesta["PuedeIngresar"] = puede;
                respuesta["Respuesta"] = "OK";
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }

        [HttpPost]
        public string AdminListarConversacion()
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

                iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"])
                );

                var lista = iAplicacion.AdminListarConversacion(reservaId);

                respuesta["Entidades"] = lista;
                respuesta["Respuesta"] = "OK";
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }

        [HttpPost]
        public string AdminEnviar()
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

                iAplicacion!.Configurar(Configuracion.ObtenerValor("StringConexion"));

                var reservaId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["ReservaId"])
                );
                var adminId = JsonConversor.ConvertirAObjeto<int>(
                    JsonConversor.ConvertirAString(datos["AdminId"])
                );
                var texto = JsonConversor.ConvertirAObjeto<string>(
                    JsonConversor.ConvertirAString(datos["Texto"])
                );

                var mensaje = iAplicacion.AdminEnviarMensaje(reservaId, adminId, texto);

                respuesta["Entidad"] = mensaje;
                respuesta["Respuesta"] = "OK";
            }
            catch (Exception ex)
            {
                respuesta["Error"] = ex.Message;
            }

            return JsonConversor.ConvertirAString(respuesta);
        }
    }
}