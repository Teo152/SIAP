

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades
{
    public class Busquedas
    {
        public int id { get; set; }
  
        public DateTime? fecha_deseada { get; set; }
        public DateTime? fecha_Fin { get; set; }

        public int? Cantidad_Huespedes { get; set; }
        
        public string? ciudad { get; set; }

        public double? precio_min    { get; set; }
        public double? precio_max    { get; set; }

        public string? tipo_propiedad { get; set; }
        public int usuario { get; set; }
        [ForeignKey("usuario")][JsonIgnore] public Usuarios _Usuario { get; set; }

    }
}
