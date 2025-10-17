
using System.ComponentModel.DataAnnotations.Schema;

using System.Text.Json.Serialization;


namespace lib_dominio.Entidades
{
    public class Pagos
    {
        public int id { get; set; }
        public DateTime fecha_pago { get; set; }
        public string codigo { get; set; }
        public double monto { get; set; }
        public string metodo { get; set; }
        public int reserva { get; set; }
        public int usuario { get; set; }

        [ForeignKey("reserva")][JsonIgnore] public Reservas _Reserva { get; set; }
        [ForeignKey("usuario")][JsonIgnore] public Usuarios _Usuario { get; set; }
    }
}
