

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades
{
    public class Mensajes
    {
        public int id { get; set; }
        public string texto { get; set; }
        public int remitente { get; set; }
        public int destinatario { get; set; }

        [ForeignKey("remitente")][JsonIgnore] public Usuarios _Usuario { get; set; }
        [ForeignKey("destinatario")][JsonIgnore] public Usuarios _Usuario1 { get; set; }
    }
}
