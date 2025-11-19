using lib_presentaciones.Interfaces;
using lib_dominio.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.MensajesPage
{
    public class IndexModel : PageModel
    {
        public class ConversacionItem
        {
            public int ReservaId { get; set; }
            public int OtroUsuarioId { get; set; }
            public string NombreOtroUsuario { get; set; } = "";
            public string DetallePropiedad { get; set; } = "";
            public int NoLeidos { get; set; }
        }

        private readonly IReservasPresentacion _reservasPresentacion;
        private readonly IUsuariosPresentacion _usuariosPresentacion;
        private readonly IMensajeriaPresentacion _mensajeriaPresentacion;

        public IndexModel(
            IReservasPresentacion reservasPresentacion,
            IUsuariosPresentacion usuariosPresentacion,
            IMensajeriaPresentacion mensajeriaPresentacion)
        {
            _reservasPresentacion = reservasPresentacion;
            _usuariosPresentacion = usuariosPresentacion;
            _mensajeriaPresentacion = mensajeriaPresentacion;
        }

        public int UsuarioActualId { get; set; }
        public string NombreUsuarioActual { get; set; } = "Tú";

        public List<ConversacionItem> Conversaciones { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            UsuarioActualId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            NombreUsuarioActual = HttpContext.Session.GetString("NombreUsuario") ?? "Tú";

            if (UsuarioActualId == 0)
                return RedirectToPage("/Login");

            var reservas = await _reservasPresentacion.ListarParaMensajeria(UsuarioActualId);

            Conversaciones = new List<ConversacionItem>();

            foreach (var r in reservas)
            {
                bool soyHuesped = r.UsuarioId == UsuarioActualId;
                int otroUsuarioId = soyHuesped
                    ? (r.Propiedad?.UsuarioId ?? 0)
                    : r.UsuarioId;

                if (otroUsuarioId == 0)
                    continue;

                var otro = await _usuariosPresentacion.PorId(otroUsuarioId);
                string nombreOtro = otro != null
                    ? $"{otro.Nombre} {otro.Apellido}".Trim()
                    : $"Usuario {otroUsuarioId}";

                string detallePropiedad = r.Propiedad?.Nombre ?? $"Reserva #{r.Id}";

                // 👇 AQUÍ: contar mensajes no leídos para esta conversación
                int noLeidos = await _mensajeriaPresentacion.ContarNoLeidos(
                    UsuarioActualId,
                    otroUsuarioId,
                    r.Id
                );

                Conversaciones.Add(new ConversacionItem
                {
                    ReservaId = r.Id,
                    OtroUsuarioId = otroUsuarioId,
                    NombreOtroUsuario = nombreOtro,
                    DetallePropiedad = detallePropiedad,
                    NoLeidos = noLeidos
                });
            }

            return Page();
        }
    }
}