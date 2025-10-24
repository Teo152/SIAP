using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages
{
    public class RegistroModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public RegistroModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public Usuarios Usuario { get; set; } = new();

        [BindProperty]
        public IFormFile? FotoPerfil { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // ? Guardar imagen si existe
                if (FotoPerfil != null)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(FotoPerfil.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await FotoPerfil.CopyToAsync(stream);
                    }

                    Usuario.Foto = "/uploads/" + uniqueFileName;
                }
                else
                {
                    // Imagen por defecto
                    Usuario.Foto = "/uploads/user_default.jpg";
                }

                // ? Establecer rol (Anfitrión)
                Usuario.Rol = RolUsuario.Anfitrion;

                // ? Guardar usuario en la base de datos
                var servicio = new UsuariosPresentacion();
                var resultado = await servicio.Guardar(Usuario);

                // ? Obtener ID del usuario recién creado
                var usuarios = await servicio.Listar();
                var nuevoUsuario = usuarios.LastOrDefault(u => u.Email == Usuario.Email);

                if (nuevoUsuario != null)
                {
                    HttpContext.Session.SetInt32("UsuarioId", nuevoUsuario.Id);
                    HttpContext.Session.SetString("Rol", nuevoUsuario.Rol.ToString());
                    HttpContext.Session.SetString("NombreUsuario", nuevoUsuario.Nombre);
                }

                // ? Redirigir a la página de anfitrión
                return RedirectToPage("/Anfitrion");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrar usuario: {ex.Message}");
                return Page();
            }
        }
    }
}
