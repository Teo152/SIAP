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

                // ? Buscamos el usuario por email (ignorando may�sculas/min�sculas)
                var usuario = usuarios.FirstOrDefault(u =>
                    u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase));

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "El correo no est� registrado.");
                    return Page();
                }

                // ? Verificamos la contrase�a
                if (usuario.Contrasena != Password)
                {
                    ModelState.AddModelError(string.Empty, "Contrase�a incorrecta.");
                    return Page();
                }

                // ? Guardamos datos del usuario en sesi�n
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("Rol", usuario.Rol.ToString());
                HttpContext.Session.SetString("NombreUsuario", usuario.Nombre);

                // ? Redirigimos seg�n el rol
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
                ModelState.AddModelError(string.Empty, $"Error al iniciar sesi�n: {ex.Message}");
                return Page();
            }
        }
    }
}
