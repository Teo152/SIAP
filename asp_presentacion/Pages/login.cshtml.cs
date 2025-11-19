using System;
using System.Linq;
using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        // Mensaje para mostrar errores en la vista
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // Servicio que obtiene los usuarios
                var servicio = new UsuariosPresentacion();
                var usuarios = await servicio.Listar(); // usamos Listar en lugar de PorEmail

                // Buscar usuario por email (ignorando mayúsculas/minúsculas)
                var usuario = usuarios.FirstOrDefault(u =>
                    u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase));

                if (usuario == null)
                {
                    ErrorMessage = "El usuario no existe";
                    return Page();
                }

                // Verificar contraseña
                if (usuario.Contrasena != Password)
                {
                    ErrorMessage = "Contraseña incorrecta";
                    return Page();
                }

                // Normalizar el nombre del rol para guardarlo en sesión
                string rolNombre;
                switch (usuario.Rol)
                {
                    case RolUsuario.Administrador:
                        rolNombre = "Admin";          // 👈 así lo usan tus páginas de Admin
                        break;

                    case RolUsuario.Anfitrion:
                        rolNombre = "Anfitrion";
                        break;

                    case RolUsuario.Huesped:
                    default:
                        rolNombre = "Huesped";
                        break;
                }

                // Guardar datos del usuario en sesión
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetInt32("Rol", (int)usuario.Rol);   // rol numérico
                HttpContext.Session.SetString("RolNombre", rolNombre);   // rol texto normalizado
                HttpContext.Session.SetString("NombreUsuario", usuario.Nombre);
                HttpContext.Session.SetString("FotoUsuario", usuario.Foto ?? "/uploads/user_default.jpg");

                // Redirigir según el rol
                switch (usuario.Rol)
                {
                    case RolUsuario.Anfitrion:
                        return RedirectToPage("/Anfitrion");

                    case RolUsuario.Administrador:
                        return RedirectToPage("/Admin/Index");

                    case RolUsuario.Huesped:
                    default:
                        return RedirectToPage("/Huesped/Huesped");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al iniciar sesión: {ex.Message}";
                return Page();
            }
        }
    }
}