using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;
        private readonly IBusquedasPresentacion _busquedasPresentacion;

        public List<Propiedades> ListaPropiedades { get; set; } = new();

        // filtros que vienen por query string desde el formulario
        [BindProperty(SupportsGet = true)]
        public string? Ciudad { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? Fecha_deseada { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? Fecha_Fin { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Cantidad_Huespedes { get; set; }

        public IndexModel(
            IPropiedadesPresentacion propiedadesPresentacion,
            IBusquedasPresentacion busquedasPresentacion)
        {
            _propiedadesPresentacion = propiedadesPresentacion;
            _busquedasPresentacion = busquedasPresentacion;
        }

        public async Task OnGetAsync()
        {
            var hayFiltros =
                !string.IsNullOrWhiteSpace(Ciudad) ||
                Fecha_deseada.HasValue ||
                Fecha_Fin.HasValue ||
                Cantidad_Huespedes.HasValue;

            if (!hayFiltros)
            {
                // sin filtros: traemos todas las propiedades
                ListaPropiedades = await _propiedadesPresentacion.Listar();
                return;
            }

            // con filtros: usamos el caso de uso de Busquedas
            var filtros = new Busquedas
            {
                Ciudad = Ciudad,
                Fecha_deseada = Fecha_deseada,
                Fecha_Fin = Fecha_Fin,
                Cantidad_Huespedes = Cantidad_Huespedes
            };

            ListaPropiedades = await _busquedasPresentacion.Filtro(filtros);
        }
    }
}
