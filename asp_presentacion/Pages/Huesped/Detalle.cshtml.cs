using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace asp_presentacion.Pages.Huesped
{
    public class DetalleModel : PageModel
    {
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;
        private readonly IReservasPresentacion _reservasPresentacion;

        public DetalleModel(
            IPropiedadesPresentacion propiedadesPresentacion,
            IReservasPresentacion reservasPresentacion)
        {
            _propiedadesPresentacion = propiedadesPresentacion;
            _reservasPresentacion = reservasPresentacion;
        }

        public Propiedades? Propiedad { get; set; }

        // ?? aquí guardamos TODAS las fechas ocupadas (yyyy-MM-dd)
        public List<string> FechasReservadas { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return RedirectToPage("./Huesped");

            // 1. Buscar la propiedad por nombre (como ya lo hacías)
            var lista = await _propiedadesPresentacion.PorNombre(
                new Propiedades { Nombre = nombre }
            );

            Propiedad = lista.FirstOrDefault();

            if (Propiedad == null)
                return RedirectToPage("./Huesped");

            // 2. Traer reservas de esa propiedad y con EstadoId = 2
            //    Ajusta este método según tu interfaz real de reservas
            var todas = await _reservasPresentacion.Listar(); // o ListarPorPropiedad, etc.

            var reservasPropiedad = todas
                .Where(r => r.PropiedadId == Propiedad.Id && r.EstadoId == 2);

            var fechas = new HashSet<string>();

            foreach (var r in reservasPropiedad)
            {
                // supongo que tus campos son DateTime
                var inicio = r.Fecha_deseada.Date;
                var fin = r.Fecha_fin.Date;

                for (var f = inicio; f <= fin; f = f.AddDays(1))
                {
                    fechas.Add(f.ToString("yyyy-MM-dd"));
                }
            }

            FechasReservadas = fechas.ToList();

            return Page();
        }
    }
}
