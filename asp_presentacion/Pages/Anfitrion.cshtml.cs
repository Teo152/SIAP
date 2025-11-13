using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;

namespace SkyBnB.Pages.Anfitrion
{
    public class IndexModel : PageModel
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
                // Sin filtros ? por ahora todas las propiedades
                // (si luego quieres solo las del anfitrión, aquí se filtra por UsuarioId)
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
