using asp_presentacion.Hubs;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace asp_presentacion.Pages.MensajesPage
{
    public class ChatModel : PageModel
    {
        private readonly IMensajeriaPresentacion _mensajeria;
        private readonly IUsuariosPresentacion _usuariosPresentacion;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IReportesChatPresentacion _reportesChatPresentacion;

        public ChatModel(
            IMensajeriaPresentacion mensajeria,
            IUsuariosPresentacion usuariosPresentacion,
            IHubContext<ChatHub> hubContext,
            IReportesChatPresentacion reportesChatPresentacion)
        {
            _mensajeria = mensajeria;
            _usuariosPresentacion = usuariosPresentacion;
            _hubContext = hubContext;
            _reportesChatPresentacion = reportesChatPresentacion;
        }

        public int UsuarioActualId { get; set; }
        public int OtroUsuarioId { get; set; }
        public int ReservaId { get; set; }

        public string NombreUsuarioActual { get; set; } = "Tú";
        public string NombreOtroUsuario { get; set; } = "";

        public string FotoUsuarioActual { get; set; } = "/uploads/user_default.jpg";
        public string FotoOtroUsuario { get; set; } = "/uploads/user_default.jpg";

        public List<Mensajes> Conversacion { get; set; } = new();

        [BindProperty]
        public string TextoMensaje { get; set; } = "";

        [BindProperty]
        public string MotivoReporte { get; set; } = "";

        public ReporteChat? ReporteActivo { get; set; }
        public bool TieneReporteActivo { get; set; }

        public async Task<IActionResult> OnGet(int reservaId, int otroUsuarioId)
        {
            var http = HttpContext;

            UsuarioActualId = http.Session.GetInt32("UsuarioId") ?? 0;
            if (UsuarioActualId == 0)
                return RedirectToPage("/Login");

            ReservaId = reservaId;
            OtroUsuarioId = otroUsuarioId;

            NombreUsuarioActual = http.Session.GetString("NombreUsuario") ?? "Tú";
            FotoUsuarioActual = http.Session.GetString("FotoUsuario") ?? "/uploads/user_default.jpg";

            var otro = await _usuariosPresentacion.PorId(OtroUsuarioId);
            if (otro != null)
            {
                NombreOtroUsuario = $"{otro.Nombre} {otro.Apellido}".Trim();
                FotoOtroUsuario = string.IsNullOrWhiteSpace(otro.Foto)
                    ? "/uploads/user_default.jpg"
                    : otro.Foto;
            }

            await _mensajeria.MarcarComoLeidos(UsuarioActualId, OtroUsuarioId, ReservaId);

            ReporteActivo = await _reportesChatPresentacion.ObtenerActivoPorReserva(ReservaId);
            TieneReporteActivo = ReporteActivo != null;

            Conversacion = await _mensajeria.ListarConversacion(
                UsuarioActualId,
                OtroUsuarioId,
                ReservaId);

            return Page();
        }

        public async Task<IActionResult> OnPostEnviar(int reservaId, int otroUsuarioId)
        {
            var http = HttpContext;

            UsuarioActualId = http.Session.GetInt32("UsuarioId") ?? 0;
            if (UsuarioActualId == 0)
                return RedirectToPage("/Login");

            ReservaId = reservaId;
            OtroUsuarioId = otroUsuarioId;

            NombreUsuarioActual = http.Session.GetString("NombreUsuario") ?? "Tú";
            FotoUsuarioActual = http.Session.GetString("FotoUsuario") ?? "/uploads/user_default.jpg";

            var otro = await _usuariosPresentacion.PorId(OtroUsuarioId);
            if (otro != null)
            {
                NombreOtroUsuario = $"{otro.Nombre} {otro.Apellido}".Trim();
                FotoOtroUsuario = string.IsNullOrWhiteSpace(otro.Foto)
                    ? "/uploads/user_default.jpg"
                    : otro.Foto;
            }

            if (string.IsNullOrWhiteSpace(TextoMensaje))
            {
                ModelState.AddModelError("", "El mensaje no puede estar vacío.");
                Conversacion = await _mensajeria.ListarConversacion(
                    UsuarioActualId,
                    OtroUsuarioId,
                    ReservaId
                );
                return Page();
            }
            var nuevo = new Mensajes
            {
                RemitenteId = UsuarioActualId,
                DestinatarioId = OtroUsuarioId,
                Texto = TextoMensaje
            };

            var guardado = await _mensajeria.Enviar(ReservaId, nuevo);

            await _hubContext.Clients.User(OtroUsuarioId.ToString())
                .SendAsync("RecibirMensaje", new
                {
                    remitenteId = guardado.RemitenteId,
                    destinatarioId = guardado.DestinatarioId,
                    texto = guardado.Texto
                });

            await _hubContext.Clients.User(UsuarioActualId.ToString())
                .SendAsync("RecibirMensaje", new
                {
                    remitenteId = guardado.RemitenteId,
                    destinatarioId = guardado.DestinatarioId,
                    texto = guardado.Texto
                });

            await _hubContext.Clients.User(UsuarioActualId.ToString())
                .SendAsync("ActualizarConversaciones");
            await _hubContext.Clients.User(OtroUsuarioId.ToString())
                .SendAsync("ActualizarConversaciones");

            TextoMensaje = string.Empty;

            Conversacion = await _mensajeria.ListarConversacion(
                UsuarioActualId,
                OtroUsuarioId,
                ReservaId
            );

            ReporteActivo = await _reportesChatPresentacion.ObtenerActivoPorReserva(ReservaId);
            TieneReporteActivo = ReporteActivo != null;

            return Page();
        }

        public async Task<IActionResult> OnPostReportar(int reservaId, int otroUsuarioId)
        {
            var http = HttpContext;
            UsuarioActualId = http.Session.GetInt32("UsuarioId") ?? 0;
            if (UsuarioActualId == 0)
                return RedirectToPage("/Login");

            ReservaId = reservaId;
            OtroUsuarioId = otroUsuarioId;

            if (string.IsNullOrWhiteSpace(MotivoReporte))
            {
                ModelState.AddModelError("", "Debes escribir un motivo para el reporte.");
                await OnGet(reservaId, otroUsuarioId);
                return Page();
            }

            await _reportesChatPresentacion.Crear(reservaId, UsuarioActualId, MotivoReporte);

            TempData["ReporteCreado"] = "Se ha enviado tu reporte al administrador.";

            return RedirectToPage(new { reservaId, otroUsuarioId });
        }
    }
}