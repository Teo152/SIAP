using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades;

public class Busquedas
{
    public int Id { get; set; }

    public DateTime? Fecha_deseada { get; set; }
    public DateTime? Fecha_Fin { get; set; }

    public int? Cantidad_Huespedes { get; set; }

    public string? Ciudad { get; set; }

    public int UsuarioId { get; set; }
    [JsonIgnore] public Usuarios Usuario { get; set; }
}