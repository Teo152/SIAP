using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Anfitrion
{
    public class PropiedadesModel : PageModel
    {
        public List<Propiedades> Propiedades { get; set; } = new();

        public async Task OnGet()
        {
            // ?? Obtener todas las propiedades (sin validar usuario todavía)
            var presentacion = new PropiedadesPresentacion();
            Propiedades = await presentacion.Listar();
        }
    }
}
