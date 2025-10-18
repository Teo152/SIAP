using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades;

public class Mensajes
{
    public int Id { get; set; }
    public string Texto { get; set; }
    public int RemitenteId { get; set; }
    public int DestinatarioId { get; set; }

    [JsonIgnore] public Usuarios Remitente { get; set; } = null!;
    [JsonIgnore] public Usuarios Destinatario { get; set; } = null!;
}