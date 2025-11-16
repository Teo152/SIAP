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

        // ===== Query string que viene desde Detalle =====
        [BindProperty(SupportsGet = true)]
        public string? Fecha_deseada { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Fecha_Fin { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PropiedadId { get; set; }

        // ===== Datos de la propiedad =====
        public Propiedades? Propiedad { get; set; }

        // ===== Form =====
        [BindProperty]
        public int Huespedes { get; set; } = 1;

        // ===== Cálculos =====
        public int Noches { get; set; }
        public decimal Subtotal { get; set; }       // precio x noches
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
            // Volver a cargar la propiedad
            var lista = await _propiedadesPresentacion.Listar();
            Propiedad = lista.FirstOrDefault(p => p.Id == PropiedadId);

            if (Propiedad == null)
            {
                ModelState.AddModelError(string.Empty, "La propiedad no existe.");
                return Page();
            }

            // Validar fechas
            DateTime fechaIni = default;
            DateTime fechaFin = default;
            if (!DateTime.TryParse(Fecha_deseada, out fechaIni) ||
                !DateTime.TryParse(Fecha_Fin, out fechaFin) ||
                fechaFin.Date <= fechaIni.Date)
            {
                ModelState.AddModelError(string.Empty, "Las fechas de la reserva no son válidas.");
            }

            // Validar huéspedes
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

            // Usuario logueado (ajusta la clave de sesión si usas otra)
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0)
            {
                ModelState.AddModelError(string.Empty, "Debes iniciar sesión para reservar.");
            }

            // Recalcular valores (Noches, Subtotal, Total)
            CalcularValores();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Construir la reserva con el costo total
            var reserva = new Reservas
            {
                PropiedadId = PropiedadId,
                UsuarioId = usuarioId,
                Cantidad_huespedes = Huespedes,
                Fecha_deseada = fechaIni,
                Fecha_fin = fechaFin,
              //  Costo_total = Total
            };

            try
            {
                var resultado = await _reservasPresentacion.Guardar(reserva);

                var reservaId = resultado?.Id ?? 0;
               // var total = resultado?.Costo_total ?? Total;

                return RedirectToPage("/Huesped/Pagos", new
                {
                    reservaId = reservaId,
                   // total = total
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
                Noches = 1; // mínimo 1 noche

            var precioNoche = (decimal)Propiedad.Precio;
            Subtotal = precioNoche * Noches;
            Total = Subtotal + Limpieza + Impuestos;
        }
    }
}
