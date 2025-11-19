using System;
using System.Linq;
using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Huesped
{
    public class CrearReservaModel : PageModel
    {
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;
        private readonly IReservasPresentacion _reservasPresentacion;

        public CrearReservaModel(
            IPropiedadesPresentacion propiedadesPresentacion,
            IReservasPresentacion reservasPresentacion)
        {
            _propiedadesPresentacion = propiedadesPresentacion;
            _reservasPresentacion = reservasPresentacion;
        }

        [BindProperty(SupportsGet = true)]
        public string? Fecha_deseada { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Fecha_Fin { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PropiedadId { get; set; }

        public Propiedades? Propiedad { get; set; }

        [BindProperty]
        public int Huespedes { get; set; } = 1;

        public int Noches { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Limpieza { get; set; } = 50000m;
        public decimal Impuestos { get; set; } = 40000m;
        public decimal Total { get; set; }

        public async Task OnGetAsync()
        {
            var lista = await _propiedadesPresentacion.Listar();
            Propiedad = lista.FirstOrDefault(p => p.Id == PropiedadId);

            CalcularValores();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var lista = await _propiedadesPresentacion.Listar();
            Propiedad = lista.FirstOrDefault(p => p.Id == PropiedadId);

            if (Propiedad == null)
            {
                ModelState.AddModelError(string.Empty, "La propiedad no existe.");
                return Page();
            }

            DateTime fechaIni = default;
            DateTime fechaFin = default;
            if (!DateTime.TryParse(Fecha_deseada, out fechaIni) ||
                !DateTime.TryParse(Fecha_Fin, out fechaFin) ||
                fechaFin.Date <= fechaIni.Date)
            {
                ModelState.AddModelError(string.Empty, "Las fechas de la reserva no son válidas.");
            }

            if (Huespedes < 1)
            {
                ModelState.AddModelError(nameof(Huespedes), "Debe haber al menos 1 huésped.");
            }
            else if (Huespedes > Propiedad.Capacidad)
            {
                ModelState.AddModelError(
                    nameof(Huespedes),
                    $"El máximo de huéspedes permitidos es {Propiedad.Capacidad}."
                );
            }

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0)
            {
                ModelState.AddModelError(string.Empty, "Debes iniciar sesión para reservar.");
            }

            CalcularValores();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var reserva = new Reservas
            {
                PropiedadId = PropiedadId,
                UsuarioId = usuarioId,
                Cantidad_huespedes = Huespedes,
                Fecha_deseada = fechaIni,
                Fecha_fin = fechaFin,
            };

            try
            {
                var resultado = await _reservasPresentacion.Guardar(reserva);

                var reservaId = resultado?.Id ?? 0;

                return RedirectToPage("/Huesped/Pagos", new
                {
                    reservaId = reservaId,
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        private void CalcularValores()
        {
            Noches = 0;
            Subtotal = 0;
            Total = 0;

            if (Propiedad == null)
                return;

            if (DateTime.TryParse(Fecha_deseada, out var ini) &&
                DateTime.TryParse(Fecha_Fin, out var fin) &&
                fin.Date > ini.Date)
            {
                Noches = (int)(fin.Date - ini.Date).TotalDays;
            }

            if (Noches <= 0)
                Noches = 1;

            var precioNoche = (decimal)Propiedad.Precio;
            Subtotal = precioNoche * Noches;
            Total = Subtotal + Limpieza + Impuestos;
        }
    }
}