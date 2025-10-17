

namespace lib_dominio.Entidades
{
    public class Usuarios
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; } 
        public string contrasena { get; set; }
        public int telefono { get; set; }

        public DateTime? fecha_nacimiento { get; set; } // solo anfitrión

        public RolUsuario rol { get; set; }

        
        
        
        public int? Edad { get; set; } // solo huésped

    }
}
