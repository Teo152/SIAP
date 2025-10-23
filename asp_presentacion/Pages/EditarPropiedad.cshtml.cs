using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages
{
    public class EditarPropiedadModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public EditarPropiedadModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public Propiedades Propiedad { get; set; } = new();

        [BindProperty]
        public IFormFile? Fotos { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var servicio = new PropiedadesPresentacion();
            var lista = await servicio.Listar();

            Propiedad = lista.FirstOrDefault(p => p.Id == id)!;

            if (Propiedad == null)
                return RedirectToPage("/Propiedades");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // ? Obtener el ID del usuario que está editando (anfitrión logueado)
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null || usuarioId == 0)
                {
                    return RedirectToPage("/Login");
                }
                Propiedad.UsuarioId = usuarioId.Value;

                var servicio = new PropiedadesPresentacion();

                // ?? Si el usuario sube una nueva imagen
                if (Fotos != null)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Fotos.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Fotos.CopyToAsync(stream);
                    }

                    // Guarda la nueva ruta
                    Propiedad.Imagen = "/uploads/" + uniqueFileName;
                }
                else
                {
                    // ?? Si no se cambió la imagen, se mantiene la anterior
                    var lista = await servicio.Listar();
                    var existente = lista.FirstOrDefault(p => p.Id == Propiedad.Id);
                    if (existente != null && !string.IsNullOrEmpty(existente.Imagen))
                    {
                        Propiedad.Imagen = existente.Imagen;
                    }
                }

                // ?? Actualiza en la base de datos
                await servicio.Modificar(Propiedad);

                // ? Mensaje visual
                TempData["Mensaje"] = "La propiedad se actualizó correctamente.";

                return RedirectToPage("/Propiedades");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar la propiedad: {ex.Message}");
                return Page();
            }
        }


    }
}
