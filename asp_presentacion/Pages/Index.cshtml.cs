using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;

        public List<Propiedades> ListaPropiedades { get; set; } = new();

        public IndexModel(IPropiedadesPresentacion propiedadesPresentacion)
        {
            _propiedadesPresentacion = propiedadesPresentacion;
        }

        public async Task OnGetAsync()
        {
            ListaPropiedades = await _propiedadesPresentacion.Listar();
        }
    }
}
