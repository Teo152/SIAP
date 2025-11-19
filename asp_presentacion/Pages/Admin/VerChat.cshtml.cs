using lib_presentaciones.Interfaces;
using lib_dominio.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Admin
{
    public class VerChatModel : PageModel
    {
        private readonly IReportesChatPresentacion _reportesChat;
        private readonly IMensajeriaPresentacion _mensajeria;
        private readonly IUsuariosPresentacion _usuarios;

        public VerChatModel(
            IReportesChatPresentacion reportesChat,
            IMensajeriaPresentacion mensajeria,
            IUsuariosPresentacion usuarios)
        {
            _reportesChat = reportesChat;
            _mensajeria = mensajeria;
            _usuarios = usuarios;
        }

        public ReporteChat? Reporte { get; set; }
        public List<Mensajes> Conversacion { get; set; } = new();

        public string PropiedadNombre { get; set; } = "";
        public string HuespedNombre { get; set; } = "";
        public string AnfitrionNombre { get; set; } = "";

        [BindProperty]
        public string TextoRespuesta { get; set; } = "";

        public int HuespedId { get; set; }
        public int AnfitrionId { get; set; }

        // ================== GET ==================
        public async Task<IActionResult> OnGet(int reporteId)
        {
            var rol = HttpContext.Session.GetString("RolNombre") ?? "";
            if (rol != "Admin")
                return RedirectToPage("/Login");

            Reporte = await _reportesChat.ObtenerPorId(reporteId);
            if (Reporte == null)
                return NotFound();

            var reserva = Reporte.Reserva;
            if (reserva == null || reserva.Propiedad == null || reserva.Propiedad.UsuarioId == null)
                return NotFound();

            HuespedId = reserva.UsuarioId;
            AnfitrionId = reserva.Propiedad.UsuarioId.Value;

            PropiedadNombre = reserva.Propiedad.Nombre ?? $"Propiedad #{reserva.PropiedadId}";

            var huesped = await _usuarios.PorId(HuespedId);
            var anfitrion = await _usuarios.PorId(AnfitrionId);

            HuespedNombre = huesped != null
                ? $"{huesped.Nombre} {huesped.Apellido}".Trim()
                : $"Huésped {HuespedId}";

            AnfitrionNombre = anfitrion != null
                ? $"{anfitrion.Nombre} {anfitrion.Apellido}".Trim()
                : $"Anfitrión {AnfitrionId}";

            Conversacion = await _mensajeria.ListarConversacion(HuespedId, AnfitrionId);

            return Page();
        }

        // ============ POST: responder como admin ============
        public async Task<IActionResult> OnPostResponder(int reporteId)
        {
            var rol = HttpContext.Session.GetString("RolNombre") ?? "";
            if (rol != "Admin")
                return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(TextoRespuesta))
            {
                ModelState.AddModelError("", "El mensaje no puede estar vacío.");
                return await OnGet(reporteId);
            }

            var reporte = await _reportesChat.ObtenerPorId(reporteId);
            if (reporte == null)
                return RedirectToPage("/Admin/Index");

            if (!reporte.Activo)
            {
                TempData["ReporteCerrado"] = "Este reporte ya está finalizado. No puedes enviar más mensajes.";
                return RedirectToPage(new { reporteId });
            }

            var reserva = reporte.Reserva!;
            var huespedId = reserva.UsuarioId;
            var anfitrionId = reserva.Propiedad!.UsuarioId!.Value;

            var mensaje = new Mensajes
            {
                RemitenteId = anfitrionId, // o huespedId, según tu flujo
                DestinatarioId = huespedId,
                Texto = "[ADMIN] " + TextoRespuesta
            };

            await _mensajeria.Enviar(reporte.ReservaId, mensaje);

            TempData["AdminMensaje"] = "Mensaje enviado al chat.";

            return RedirectToPage(new { reporteId });
        }

        // ============ POST: finalizar reporte ============
        public async Task<IActionResult> OnPostFinalizar(int reporteId)
        {
            var rol = HttpContext.Session.GetString("RolNombre") ?? "";
            if (rol != "Admin")
                return RedirectToPage("/Login");

            await _reportesChat.Finalizar(reporteId);

            TempData["ReporteCerrado"] = "El reporte ha sido finalizado correctamente.";

            return RedirectToPage(new { reporteId });
        }
    }
}