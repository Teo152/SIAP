using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
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

        // ?? Propiedad para mostrar mensajes en el frontend
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
                // ? Instanciamos el servicio que obtiene los usuarios
                var servicio = new UsuariosPresentacion();
                var usuarios = await servicio.Listar(); // usamos Listar en lugar de PorEmail

                // ? Buscamos el usuario por email (ignorando mayúsculas/minúsculas)
                var usuario = usuarios.FirstOrDefault(u =>
                    u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase));

                if (usuario == null)
                {
                    ErrorMessage = "El usuario no existe";
                    return Page();
                }

                // ? Verificamos la contraseña
                if (usuario.Contrasena != Password)
                {
                    ErrorMessage = "Contraseña incorrecta";
                    return Page();
                }

                // ? Guardamos datos del usuario en sesión
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("Rol", usuario.Rol.ToString());
                HttpContext.Session.SetString("NombreUsuario", usuario.Nombre);
                // ?? Guardar la foto del usuario en la sesión
                HttpContext.Session.SetString("FotoUsuario", usuario.Foto ?? "/uploads/user_default.jpg");


                // ? Redirigimos según el rol
                switch (usuario.Rol)
                {
                    case RolUsuario.Anfitrion:
                        return RedirectToPage("/Anfitrion");

                    case RolUsuario.Administrador:
                        return RedirectToPage("/Admin/Index");

                    case RolUsuario.Huesped:
                    default:
                        return RedirectToPage("/Index");
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
