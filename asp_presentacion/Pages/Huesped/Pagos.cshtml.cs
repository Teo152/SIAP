

using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Huesped
{
    public class PagosModel : PageModel
    {
        private readonly IPagosPresentacion _pagosPresentacion;

        public PagosModel(IPagosPresentacion pagosPresentacion)
        {
            _pagosPresentacion = pagosPresentacion;
        }

        // Llega desde la reserva
        [BindProperty(SupportsGet = true)]
        public int ReservaId { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal Total { get; set; }

        // Campos del formulario
        [BindProperty]
        public string Metodo { get; set; } = "Tarjeta";

        [BindProperty]
        public string NumeroTarjeta { get; set; } = "";

        [BindProperty]
        public DateTime FechaExpiracion { get; set; } 

        [BindProperty]
        public string Cvv { get; set; } = "";

        [BindProperty]
        public string Titular { get; set; } = "";

        public void OnGet()
        {
            // Solo muestra la pantalla con el total y la reservaId
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(NumeroTarjeta) ||
                NumeroTarjeta.Replace(" ", "").Length != 16)
            {
                ModelState.AddModelError(nameof(NumeroTarjeta),
                    "El número de tarjeta debe tener 16 dígitos.");
            }

            if (string.IsNullOrWhiteSpace(Cvv) || Cvv.Length < 3)
            {
                ModelState.AddModelError(nameof(Cvv), "CVV inválido.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0)
            {
                ModelState.AddModelError(string.Empty, "Debes iniciar sesión para pagar.");
                return Page();
            }

            // Construimos la entidad Pagoss que necesita tu PagossAplicacion
            var pago = new Pagos
            {
                UsuarioId = usuarioId,
                ReservaId = ReservaId,
                Numero_targeta = NumeroTarjeta.Replace(" ", ""),
                Cvv = Cvv,
                Fecha_expiracion=FechaExpiracion,
                Nombre_Apellidos=Titular

                // puedes guardar más campos si tu entidad los tiene
            };

            try
            {
                var respuesta = await _pagosPresentacion.Procesar_Pago(pago);

                TempData["MensajePago"] = respuesta; // "Pago exitoso"
                // Redirigir a Mis Reservas o a una pantalla de confirmación
                return RedirectToPage("/Huesped/Huesped");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
