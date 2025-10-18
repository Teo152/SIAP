using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace lib_dominio.Entidades;

public class Usuarios : IdentityUser
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public DateTime FechaNacimiento { get; set; }
    public string? Foto { get; set; }
    public int RolId { get; set; }
    public RolUsuario Rol { get; set; }

    public string NombreCompleto => $"{Nombre} {Apellido}";
}