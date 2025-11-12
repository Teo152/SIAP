using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades;

public class Pagos
{
    public int Id { get; set; }
    public DateTime Fecha_pago { get; set; }
    public string Codigo { get; set; } = null!;
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Monto { get; set; }
    public string Metodo { get; set; } = null!;
    public int ReservaId { get; set; }
    public int UsuarioId { get; set; }

    [JsonIgnore] public Reservas Reserva { get; set; } = null!;
    [JsonIgnore] public Usuarios Usuario { get; set; } = null!;
}