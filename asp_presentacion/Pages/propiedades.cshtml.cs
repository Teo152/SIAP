using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Anfitrion
{
    public class PropiedadesModel : PageModel
    {
        public List<Propiedades> Propiedades { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                // ? Recuperamos el ID del usuario en sesión
                int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

                if (usuarioId == null || usuarioId == 0)
                {
                    // Si no hay usuario logueado, redirige al login
                    Response.Redirect("/Login");
                    return;
                }

                // ? Obtenemos todas las propiedades
                var presentacion = new PropiedadesPresentacion();
                var todas = await presentacion.Listar();

                // ? Filtramos solo las del usuario actual
                Propiedades = todas
                    .Where(p => p.UsuarioId == usuarioId)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"?? Error al obtener propiedades: {ex.Message}");
                Propiedades = new List<Propiedades>();
            }
        }

        // ??? Nueva función: Eliminar propiedad
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                var servicio = new PropiedadesPresentacion();

                // Llamamos al método que elimina en la capa de presentación
                await servicio.Borrar(new Propiedades { Id = id });

                // Mensaje temporal para mostrar en la vista
                TempData["Mensaje"] = "La propiedad fue eliminada correctamente.";

                // Redirige nuevamente a la misma página para recargar la lista
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al eliminar la propiedad: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
}
