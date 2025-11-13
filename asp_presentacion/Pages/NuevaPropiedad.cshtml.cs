using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
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
        private readonly IMunicipiosPresentacion _municipiosPresentacion;
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;

        public NuevaPropiedadModel(
            IWebHostEnvironment environment,
            IMunicipiosPresentacion municipiosPresentacion,
            IPropiedadesPresentacion propiedadesPresentacion)
        {
            _environment = environment;
            _municipiosPresentacion = municipiosPresentacion;
            _propiedadesPresentacion = propiedadesPresentacion;
        }

        [BindProperty]
        public Propiedades Propiedad { get; set; } = new();

        [BindProperty]
        public List<IFormFile>? Fotos { get; set; }

        // ?? Lista de municipios para el dropdown
        public List<Municipios> ListaMunicipios { get; set; } = new();

        // ?? Valor seleccionado desde el formulario
        [BindProperty]
        public int? MunicipioSeleccionado { get; set; }

        public async Task OnGet()
        {
            // Cargar municipios desde la BD / API
            ListaMunicipios = await _municipiosPresentacion.Listar();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Volver a cargar municipios por si hay errores
            ListaMunicipios = await _municipiosPresentacion.Listar();

            if (!ModelState.IsValid)
                return Page();

            try
            {
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                if (usuarioId == null)
                    return RedirectToPage("/Login");

                Propiedad.UsuarioId = usuarioId.Value;

                // ?? VALIDACIÓN DEL MUNICIPIO
                if (MunicipioSeleccionado == null || MunicipioSeleccionado == 0)
                {
                    ModelState.AddModelError("MunicipioSeleccionado", "Debe seleccionar un municipio.");
                    return Page();
                }

                var municipio = ListaMunicipios.FirstOrDefault(m => m.Id == MunicipioSeleccionado);
                if (municipio == null)
                {
                    ModelState.AddModelError("MunicipioSeleccionado", "El municipio seleccionado no existe.");
                    return Page();
                }

                // Asignar el MunicipioId a la propiedad
                Propiedad.MunicipioId = municipio.Id;

                // ---- VALIDAR DIRECCIÓN EXISTENTE ----
                var todas = await _propiedadesPresentacion.Listar();

                bool direccionExiste = todas.Any(p =>
                    p.Direccion.Trim().ToLower() == Propiedad.Direccion.Trim().ToLower());

                if (direccionExiste)
                {
                    ModelState.AddModelError("Propiedad.Direccion", "Ya existe una propiedad registrada en esta dirección.");
                    return Page();
                }

                // ---- SUBIR FOTOS ----
                if (Fotos != null && Fotos.Count > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var primeraFoto = Fotos.First();
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(primeraFoto.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await primeraFoto.CopyToAsync(stream);

                    Propiedad.Imagen = "/uploads/" + uniqueFileName;

                    // Subir fotos adicionales (opcional)
                    for (int i = 1; i < Fotos.Count; i++)
                    {
                        var extra = Fotos[i];
                        var extraName = Guid.NewGuid().ToString() + Path.GetExtension(extra.FileName);
                        var extraPath = Path.Combine(uploadsFolder, extraName);

                        using (var stream = new FileStream(extraPath, FileMode.Create))
                            await extra.CopyToAsync(stream);
                    }
                }
                else
                {
                    Propiedad.Imagen = "/uploads/sin_imagen.jpg";
                }

                // ---- GUARDAR ----
                await _propiedadesPresentacion.Guardar(Propiedad);

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
