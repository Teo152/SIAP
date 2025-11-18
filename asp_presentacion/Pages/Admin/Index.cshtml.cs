using lib_presentaciones.Interfaces;
using lib_dominio.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Admin.Reportes
{
    public class IndexModel : PageModel
    {
        private readonly IReportesChatPresentacion _reportesChat;

        public IndexModel(IReportesChatPresentacion reportesChat)
        {
            _reportesChat = reportesChat;
        }

        public class ReporteItem
        {
            public int Id { get; set; }
            public int ReservaId { get; set; }
            public string PropiedadNombre { get; set; } = "";
            public string ReportanteNombre { get; set; } = "";
            public DateTime FechaCreacion { get; set; }
            public string Motivo { get; set; } = "";
        }

        public List<ReporteItem> Reportes { get; set; } = new();

        public async Task<IActionResult> OnGet()
        {
            // Solo Admin puede ver esta pantalla
            var rol = HttpContext.Session.GetString("RolNombre") ?? "";
            if (rol != "Admin")
                return RedirectToPage("/Login");

            var lista = await _reportesChat.ListarActivos();

            Reportes = lista.Select(r => new ReporteItem
            {
                Id = r.Id,
                ReservaId = r.ReservaId,
                // Ajusta según tu entidad Propiedad:
                // NombrePropiedad, Nombre, Titulo, etc.
                PropiedadNombre = r.Reserva?.Propiedad?.Nombre
                                  ?? r.Reserva?.Propiedad?.Nombre
                                  ?? $"Reserva #{r.ReservaId}",

                ReportanteNombre = r.UsuarioReportante != null
                    ? $"{r.UsuarioReportante.Nombre} {r.UsuarioReportante.Apellido}".Trim()
                    : $"Usuario {r.UsuarioReportanteId}",

                FechaCreacion = r.FechaCreacion,
                Motivo = r.Motivo ?? ""
            }).ToList();

            return Page();
        }
    }
}