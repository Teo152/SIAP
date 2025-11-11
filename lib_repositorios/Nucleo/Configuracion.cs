using lib_dominio.Nucleo;

namespace lib_repositorios.Nucleo
{
    public class Configuracion
    {
        private Dictionary<string, string>? datos = null;

        public string ObtenerValor(string clave)
        {
            string respuesta = "";
            if (datos == null)
                Cargar();
            respuesta = datos![clave].ToString();
            return respuesta;
        }

        public void Cargar()
        {
            if (!File.Exists(DatosGenerales.ruta_json))
                return;
            datos = new Dictionary<string, string>();
            StreamReader jsonStream = File.OpenText(DatosGenerales.ruta_json);
            var json = jsonStream.ReadToEnd();
            datos = JsonConversor.ConvertirAObjeto<Dictionary<string, string>>(json)!;
        }
    }
}