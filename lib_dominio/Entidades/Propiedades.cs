

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades
{
    public class Propiedades
    {
        public int id { get; set; }
        public string nombre { get; set; }
        
        public string direccion { get; set; }

        public int capacidad { get; set; }

        public string tipo_de_propiedad { get; set; }

        public decimal precio { get; set; } //precio por noche

        //public bool? disponible { get; set; }
        public string estanciaminima { get; set; }

        public string? reglas_propiedad { get; set; }
        public string? descripcion { get; set; }
        public string politicas_cancelacion { get; set; }
        public int? usuario { get; set; }
       
        
        public string? imagen { get; set; }
        // Relación con Usuarios
        // Un anfitrión puede tener múltiples propiedades
        [ForeignKey("usuario")][JsonIgnore] public Usuarios _Usuario { get; set; }

    
       
    }
}
