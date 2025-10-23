using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades;

public class Reservas
{
    public int Id { get; set; }

    public DateTime Fecha_creacion { get; set; }
    public DateTime Fecha_deseada { get; set; }
    public DateTime Fecha_fin { get; set; }
    public int Cantidad_huespedes { get; set; }
    public double Costo_total { get; set; }
    public int EstadoId { get; set; }
    public int PropiedadId { get; set; }
    public int UsuarioId { get; set; }
    //public Estados Estado { get; set; } preguntar por esto

    [JsonIgnore] public Propiedades Propiedad { get; set; } = null!;
    [JsonIgnore] public Usuarios Usuario { get; set; } = null!;

    [JsonIgnore] public ICollection<Resenas>? Resenas { get; set; }
}