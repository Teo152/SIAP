using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_dominio.Nucleo;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Huesped
{
    public class ReservasModel : PageModel
    {
        private readonly IReservasPresentacion _reservasPresentacion;
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;
        private readonly IUsuariosPresentacion _usuariosPresentacion;

        public ReservasModel(
            IReservasPresentacion reservasPresentacion,
            IPropiedadesPresentacion propiedadesPresentacion,
            IUsuariosPresentacion usuariosPresentacion)
        {
            _reservasPresentacion = reservasPresentacion;
            _propiedadesPresentacion = propiedadesPresentacion;
            _usuariosPresentacion = usuariosPresentacion;
        }

        public List<ReservaItemVm> Reservas { get; set; } = new();

        public async Task OnGetAsync()
        {


            try
            {
                await _reservasPresentacion.ActualizarEstados();
            }
            catch
            {
                // si falla, solo no actualiza estados, pero la página sigue
            }

            // 2?? Ahora sí, trae las reservas ya actualizadas
            await CargarReservasAsync();
        

           
        }

        private async Task CargarReservasAsync()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0)
            {
                // si no hay sesión, simplemente no mostramos nada
                return;
            }

            // 1. Todas las reservas
            var reservas = await _reservasPresentacion.Listar();

            // Solo las del huésped logueado y, si quieres, solo pagadas / futuras
            var reservasUsuario = reservas
                .Where(r => r.UsuarioId == usuarioId)
                .Where(r => r.EstadoId == 2) // 2 = confirmada/pagada (ajusta según tu enum)
                .ToList();

            if (!reservasUsuario.Any())
                return;

            // 2. Propiedades y usuarios
            var propiedades = await _propiedadesPresentacion.Listar();
            var usuarios = await _usuariosPresentacion.Listar();

            foreach (var r in reservasUsuario)
            {
                var prop = propiedades.FirstOrDefault(p => p.Id == r.PropiedadId);
                var anfitrionUsuario = prop != null
                    ? usuarios.FirstOrDefault(u => u.Id == prop.UsuarioId && u.Rol == RolUsuario.Anfitrion)
                    : null;

                var noches = (r.Fecha_fin.Date - r.Fecha_deseada.Date).Days;

                Reservas.Add(new ReservaItemVm
                {
                    ReservaId = r.Id,
                    PropiedadNombre = prop?.Nombre ?? $"Propiedad #{r.PropiedadId}",
                    Ciudad = prop?.Direccion ?? "Sin dirección",
                    AnfitrionNombre = anfitrionUsuario != null
                        ? $"{anfitrionUsuario.Nombre} {anfitrionUsuario.Apellido}"
                        : "Anfitrión",
                    CheckIn = r.Fecha_deseada,
                    CheckOut = r.Fecha_fin,
                    Noches = noches,
                    ImagenUrl = string.IsNullOrEmpty(prop?.Imagen)
                        ? "/uploads/sin_imagen.jpg"
                        : prop.Imagen!
                });
            }
        }

        // ?? POST: eliminar reserva (solo si faltan al menos 8 días)
        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId == 0)
            {
                TempData["ErrorReserva"] = "Debes iniciar sesión para gestionar tus reservas.";
                return RedirectToPage("/Huesped/Reservas");
            }

            var reservas = await _reservasPresentacion.Listar();
            var reserva = reservas.FirstOrDefault(r => r.Id == id && r.UsuarioId == usuarioId);

            if (reserva == null)
            {
                TempData["ErrorReserva"] = "No se encontró la reserva.";
                return RedirectToPage("/Huesped/Reservas");
            }

            // Regla: solo se puede eliminar hasta 8 días antes del check-in
            var diasRestantes = (reserva.Fecha_deseada.Date - DateTime.Today).TotalDays;

            if (diasRestantes < 8)
            {
                TempData["ErrorReserva"] =
                    "Solo puedes cancelar la reserva hasta 8 días antes de la fecha de check-in.";
                return RedirectToPage("/Huesped/Reservas");
            }

            try
            {
                // ?? marcar como cancelada (ajusta el número según tu catálogo)
                reserva.EstadoId = 4; // 4 = Cancelada (por ejemplo)

                await _reservasPresentacion.Modificar(reserva);

                TempData["MensajeReserva"] = "La reserva se canceló correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorReserva"] = $"Error al cancelar la reserva: {ex.Message}";
            }

            return RedirectToPage("/Huesped/Reservas");
        }

    }

    public class ReservaItemVm
    {
        public int ReservaId { get; set; }
        public string PropiedadNombre { get; set; } = "";
        public string AnfitrionNombre { get; set; } = "";
        public string Ciudad { get; set; } = "";
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Noches { get; set; }
        public string ImagenUrl { get; set; } = "";
    }
}
