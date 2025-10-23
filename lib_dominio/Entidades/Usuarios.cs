

namespace lib_dominio.Entidades
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; } 
        public string Contrasena { get; set; }
        public int Telefono { get; set; }

        public string? Foto { get; set; }
        public DateTime? Fecha_nacimiento { get; set; } // solo anfitrión

        public RolUsuario? Rol { get; set; }

        
        
        
        
    }
}
