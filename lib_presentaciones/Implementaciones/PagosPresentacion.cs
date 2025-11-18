using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using lib_presentaciones.Interfaces;

namespace lib_presentaciones.Implementaciones
{
    public class PagosPresentacion : IPagosPresentacion
    {
        private Comunicaciones? comunicaciones;

        public async Task<string> Procesar_Pago(Pagos entidad)

        {

            if (entidad!.Id != 0)
            {
                throw new Exception("lbFaltaInformacion");
            }

            var datos = new Dictionary<string, object>();
            datos["Entidad"] = entidad;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Pagos/Procesar_pago");

            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"].ToString()!);

            return respuesta["Respuesta"].ToString()!;
        }

        public async Task<string> GenerarFactura(Pagos entidad)
        {
            if (entidad == null)
                throw new Exception("No se puede generar factura sin pago.");

            var datos = new Dictionary<string, object>();
            datos["Entidad"] = entidad;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Pagos/Generar_factura");

            var respuesta = await comunicaciones.Execute(datos);

            if (respuesta.ContainsKey("Error"))
                throw new Exception(respuesta["Error"].ToString()!);

            // 👇 aquí estaba el problema: antes usabas "Respuesta"
            if (!respuesta.ContainsKey("Factura"))
                throw new Exception("El servicio Pagos/Generar_factura no devolvió la clave 'Factura'.");

            var factura = respuesta["Factura"]!.ToString()!;
            return factura;
        }


        public async Task<Propiedades?> Guardar(Propiedades? entidad)
        {
            if (entidad!.Id != 0)
            {
                throw new Exception("lbFaltaInformacion");
            }

            var datos = new Dictionary<string, object>();
            datos["Entidad"] = entidad;

            comunicaciones = new Comunicaciones();
            datos = comunicaciones.ConstruirUrl(datos, "Propiedades/Guardar");
            var respuesta = await comunicaciones!.Execute(datos);

            if (respuesta.ContainsKey("Error"))
            {
                throw new Exception(respuesta["Error"].ToString()!);
            }
            entidad = JsonConversor.ConvertirAObjeto<Propiedades>(
                JsonConversor.ConvertirAString(respuesta["Entidad"]));
            return entidad;
        }
    }
}

