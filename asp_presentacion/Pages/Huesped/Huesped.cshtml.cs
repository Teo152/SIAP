using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;

namespace asp_presentacion.Pages.Huesped
{
    public class HuespedModel : PageModel
    {
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;
        private readonly IBusquedasPresentacion _busquedasPresentacion;

        public List<Propiedades> Propiedades { get; set; } = new();

        // Filtros (se llenan desde el formulario por query string)
        [BindProperty(SupportsGet = true)]
        public string? Ciudad { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? Fecha_deseada { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? Fecha_Fin { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Cantidad_Huespedes { get; set; }

        public HuespedModel(
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
                // Sin filtros: mostramos todas las propiedades
                Propiedades = await _propiedadesPresentacion.Listar();
                return;
            }

            var filtros = new Busquedas
            {
                Ciudad = Ciudad,
                Fecha_deseada = Fecha_deseada,
                Fecha_Fin = Fecha_Fin,
                Cantidad_Huespedes = Cantidad_Huespedes
            };

            Propiedades = await _busquedasPresentacion.Filtro(filtros);
        }
    }
}
