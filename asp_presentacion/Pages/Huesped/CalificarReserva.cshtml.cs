// asp_presentacion/Pages/Huesped/CalificarReserva.cshtml.cs
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Huesped
{
    public class CalificarReservaModel : PageModel
    {
        private readonly IReservasPresentacion _reservas;
        private readonly IResenasPresentacion _resenas;
        private readonly IPropiedadesPresentacion _propiedades;

        public CalificarReservaModel(
            IReservasPresentacion reservas,
            IResenasPresentacion resenas,
            IPropiedadesPresentacion propiedades)
        {
            _reservas = reservas;
            _resenas = resenas;
            _propiedades = propiedades;
        }

        public Reservas? Reserva { get; set; }
        public Propiedades? Propiedad { get; set; }

        [BindProperty]
        public int Calificacion { get; set; }

        [BindProperty]
        public string Comentario { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0)
                return RedirectToPage("/Login");

            var reservas = await _reservas.Listar();
            Reserva = reservas.FirstOrDefault(r => r.Id == id);
            if (Reserva == null)
                return RedirectToPage("/Huesped/Reservas");

            if (Reserva.UsuarioId != usuarioId)
                return RedirectToPage("/Huesped/Reservas");

            if (Reserva.EstadoId != 5) // finalizada
                return RedirectToPage("/Huesped/Reservas");

            // ¿ya tiene reseña?
            var existente = await _resenas.ObtenerPorReserva(id);
            if (existente != null)
            {
                TempData["MensajeReserva"] = "Esta reserva ya tiene una reseña registrada.";
                return RedirectToPage("/Huesped/Reservas");
            }

            var props = await _propiedades.Listar();
            Propiedad = props.FirstOrDefault(p => p.Id == Reserva.PropiedadId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0)
                return RedirectToPage("/Login");

            // Validar rango de calificación
            if (Calificacion < 1 || Calificacion > 5)
            {
                ModelState.AddModelError(nameof(Calificacion),
                    "La calificación debe estar entre 1 y 5.");
            }

            if (!ModelState.IsValid)
                return Page();

            // 1?? Cargar la reserva por id
            var reservas = await _reservas.Listar();
            Reserva = reservas.FirstOrDefault(r => r.Id == id);
            if (Reserva == null)
                return RedirectToPage("/Huesped/Reservas");

            // 2?? Construir y guardar reseña
            var reseña = new Resenas
            {
                ReservaId = Reserva.Id,
                PropiedadId = Reserva.PropiedadId,
                Calificacion = Calificacion,
                Comentario = Comentario ?? ""
            };

            await _resenas.Guardar(reseña, usuarioId);

            TempData["MensajeReserva"] = "¡Gracias! Tu reseña ha sido registrada.";

            // 3?? Volver a cargar la propiedad (Propiedad viene null en el POST)
            var props = await _propiedades.Listar();
            var propiedad = props.FirstOrDefault(p => p.Id == Reserva.PropiedadId);

            if (propiedad == null)
                return RedirectToPage("/Huesped/Reservas");

            // 4?? Redirigir al detalle usando el nombre de la propiedad
            return RedirectToPage("/Huesped/Detalle", new { nombre = propiedad.Nombre });
        }

    }
}
