
using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json.Serialization;


namespace lib_dominio.Entidades
{
    public class Resenas
    {
        public int id { get; set; }
        public int calificacion { get; set; }
        public string comentario { get; set; }
        public DateTime fecha_creacion { get; set; }
        public int propiedad { get; set; }
        public int reserva { get; set; }

        [ForeignKey("propiedad")][JsonIgnore] public Propiedades _Propiedad { get; set; }
        [ForeignKey("reserva")][JsonIgnore] public Reservas _Reserva { get; set; }

    }
}
