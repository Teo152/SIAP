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

        // 1 = Anfitrión, 2 = Huésped (viene de /Registro?rol=1 o ?rol=2)
        [BindProperty(SupportsGet = true)]
        public int Rol { get; set; }

        public void OnGet()
        {
            // Aquí podrías cambiar el título dinámicamente si quieres,
            // pero lo importante es que Rol ya queda con el valor de la query (?rol=1 o 2)
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // Guardar imagen si existe
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
                    Usuario.Foto = "/uploads/user_default.jpg";
                }

                // ? Asignar el rol según lo que vino de /Registro?rol=...
                // Suponiendo tu enum:
                // public enum RolUsuario { Anfitrion = 1, Huesped = 2 }
                if (Rol == 1)
                {
                    Usuario.Rol = RolUsuario.Anfitrion;
                }
                else if (Rol == 2)
                {
                    Usuario.Rol = RolUsuario.Huesped;
                }
                else
                {
                    // Por si acaso: default huésped
                    Usuario.Rol = RolUsuario.Huesped;
                }

                // Guardar usuario en la base de datos
                var servicio = new UsuariosPresentacion();
                var resultado = await servicio.Guardar(Usuario);

                // Obtener ID del usuario recién creado
                var usuarios = await servicio.Listar();
                var nuevoUsuario = usuarios.LastOrDefault(u => u.Email == Usuario.Email);

                if (nuevoUsuario != null)
                {
                    HttpContext.Session.SetInt32("UsuarioId", nuevoUsuario.Id);
                    HttpContext.Session.SetString("Rol", nuevoUsuario.Rol.ToString());
                    HttpContext.Session.SetString("NombreUsuario", nuevoUsuario.Nombre);
                }

                // Redirigir:
                // Si es anfitrión ? página de anfitrión
                // Si es huésped ? podrías mandarlo al home o a otra página
                if (Usuario.Rol == RolUsuario.Anfitrion)
                    return RedirectToPage("/Anfitrion");
                else
                    return RedirectToPage("/Huesped/Huesped");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al registrar usuario: {ex.Message}");
                return Page();
            }
        }
    }
}
