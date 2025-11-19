using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace asp_presentacion.Pages.Huesped
{
    public class PagosModel : PageModel
    {
        private readonly IPagosPresentacion _pagosPresentacion;
        private readonly IReservasPresentacion _reservasPresentacion;
        public PagosModel(IPagosPresentacion pagosPresentacion,
                          IReservasPresentacion reservasPresentacion)
        {
            _pagosPresentacion = pagosPresentacion;
            _reservasPresentacion = reservasPresentacion;
        }

        // Llega desde la reserva (query string)
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
            // Solo muestra la pantalla con el total y la ReservaId
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Normalizar n�mero
            var numero = (NumeroTarjeta ?? string.Empty).Replace(" ", "");

            // Validaciones b�sicas
            if (string.IsNullOrWhiteSpace(numero) || numero.Length != 16 || !numero.All(char.IsDigit))
            {
                ModelState.AddModelError(nameof(NumeroTarjeta),
                    "El numero de tarjeta debe tener 16 digitos numericos.");
            }

            if (string.IsNullOrWhiteSpace(Cvv) || Cvv.Length < 3)
            {
                ModelState.AddModelError(nameof(Cvv), "CVV invalido.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0)
            {
                ModelState.AddModelError(string.Empty, "Debes iniciar sesi�n para pagar.");
                return Page();
            }

            // Construimos la entidad Pagos que necesita tu PagosAplicacion
            var pago = new Pagos
            {
                UsuarioId = usuarioId,
                ReservaId = ReservaId,
                Numero_targeta = numero,
                Cvv = Cvv,
                Nombre_Apellidos = Titular,
                Fecha_expiracion = FechaExpiracion,
                precio = Total,
                Fecha_pago = DateTime.Now,
                
                // ?? importante: la columna en BD es NOT NULL
                // agrega aqu� otros campos NOT NULL que tu entidad tenga
            };

            try
            {

                var respuestaPago = await _pagosPresentacion.Procesar_Pago(pago);
                var factura = await _pagosPresentacion.GenerarFactura(pago);


                // 3) Redirigir a p�gina de confirmaci�n
                return RedirectToPage("/Huesped/ConfirmacionReserva", new
                {
                    reservaId = ReservaId,
                    total = Total,
                    factura = factura
                    
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}