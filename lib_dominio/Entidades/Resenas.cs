using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades;

public class Resenas
{
    public int Id { get; set; }
    public int Calificacion { get; set; }
    public string Comentario { get; set; }
    public DateTime Fecha_creacion { get; set; }
    public int PropiedadId { get; set; }
    public int ReservaId { get; set; }

    // 🔸 Propiedades de navegación
    [JsonIgnore] public Reservas Reserva { get; set; } = null!;
    [JsonIgnore] public Propiedades Propiedad { get; set; } = null!;

    //  [NotMapped]
    // public Propiedades Propiedad => Reserva.Propiedad;
}