using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades;

public class Pagos
{
    public int Id { get; set; }
    public DateTime Fecha_pago { get; set; }
    public string Numero_targeta { get; set; } 
    [Column(TypeName = "decimal(18, 2)")]
    public decimal? precio { get; set; }
    public string Cvv { get; set; } = null!;

    public DateTime Fecha_expiracion { get; set; }

    public string Nombre_Apellidos { get; set; } = null!;


    public int MetodoId { get; set; } 
    public int ReservaId { get; set; }
    public int UsuarioId { get; set; }


    [JsonIgnore] public Reservas Reserva { get; set; } = null!;
    [JsonIgnore] public Usuarios Usuario { get; set; } = null!;
}