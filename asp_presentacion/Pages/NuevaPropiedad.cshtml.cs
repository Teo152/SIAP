using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace asp_presentacion.Pages
{
    public class NuevaPropiedadModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public NuevaPropiedadModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public Propiedades Propiedad { get; set; } = new();

        [BindProperty]
        public List<IFormFile>? Fotos { get; set; }

        public void OnGet()
        {
            // Carga inicial del formulario (vacío)
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                // ? Id del usuario logueado
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null || usuarioId == 0)
                {
                    return RedirectToPage("/Login");
                }

                Propiedad.UsuarioId = usuarioId.Value;

                // ? Verificar si ya existe una propiedad con la misma dirección
                var servicio = new PropiedadesPresentacion();
                var todas = await servicio.Listar();

                bool direccionExiste = todas.Any(p =>
                    p.Direccion.Trim().ToLower() == Propiedad.Direccion.Trim().ToLower());

                if (direccionExiste)
                {
                    ModelState.AddModelError("Propiedad.Direccion", "Ya existe una propiedad registrada en esta dirección.");
                    return Page();
                }

                // ? Si se subieron fotos
                if (Fotos != null && Fotos.Count > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                    // Crear carpeta si no existe
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Guardar la primera imagen
                    var primeraFoto = Fotos.First();
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(primeraFoto.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await primeraFoto.CopyToAsync(stream);
                    }

                    // Guardar la ruta relativa
                    Propiedad.Imagen = "/uploads/" + uniqueFileName;

                    // (Opcional) guardar más imágenes
                    for (int i = 1; i < Fotos.Count; i++)
                    {
                        var file = Fotos[i];
                        var extraName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var extraPath = Path.Combine(uploadsFolder, extraName);
                        using (var stream = new FileStream(extraPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                }
                else
                {
                    Propiedad.Imagen = "/uploads/sin_imagen.jpg";
                }

                // ? Guardar la nueva propiedad
                await servicio.Guardar(Propiedad);

                // Redirigir a la lista
                return RedirectToPage("/Propiedades");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al guardar la propiedad: {ex.Message}");
                return Page();
            }
        }
    }
}
