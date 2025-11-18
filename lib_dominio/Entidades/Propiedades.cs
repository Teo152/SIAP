using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades;

public class Propiedades
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public int Capacidad { get; set; }
    public string TipoPropiedad { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Precio { get; set; }

    //public bool? disponible { get; set; }
    public int EstanciaMinima { get; set; }

    public string? ReglasPropiedad { get; set; }
    public string? Descripcion { get; set; }
    public string PoliticasCancelacion { get; set; } = null!;
    public int? UsuarioId { get; set; }
    public int? MunicipioId { get; set; }
    public string? Imagen { get; set; }

    [ForeignKey("UsuarioId")][JsonIgnore] public Usuarios? _Usuarios { get; set; }
    [ForeignKey("MunicipioId")][JsonIgnore] public Municipios? _Municipios { get; set; }
}