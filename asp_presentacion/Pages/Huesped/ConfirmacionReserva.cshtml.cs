using System;
using System.Linq;
using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Huesped
{
    public class ConfirmacionReservaModel : PageModel
    {
        private readonly IReservasPresentacion _reservasPresentacion;
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;
        private readonly IUsuariosPresentacion _usuariosPresentacion;

        public ConfirmacionReservaModel(
            IReservasPresentacion reservasPresentacion,
            IPropiedadesPresentacion propiedadesPresentacion,
            IUsuariosPresentacion usuariosPresentacion)
        {
            _reservasPresentacion = reservasPresentacion;
            _propiedadesPresentacion = propiedadesPresentacion;
            _usuariosPresentacion = usuariosPresentacion;
        }

        // Vienen desde Pagos
        [BindProperty(SupportsGet = true)]
        public int ReservaId { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal Total { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Factura { get; set; } = string.Empty;

        // Lo que mostramos en la vista
        public string Anfitrion { get; set; } = string.Empty;
        public string Huesped { get; set; } = string.Empty;
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        public async Task OnGetAsync()
        {
            // 1. Buscar la reserva por Id
            var reservas = await _reservasPresentacion.Listar();
            var reserva = reservas.FirstOrDefault(r => r.Id == ReservaId);
            if (reserva == null)
                return;

            // Ajusta nombres a tu entidad Reservas
            CheckIn = reserva.Fecha_deseada;
            CheckOut = reserva.Fecha_fin;
            Total = (decimal)reserva.Costo_total;

            // 2. Buscar la propiedad asociada
            var propiedades = await _propiedadesPresentacion.Listar();
            var propiedad = propiedades.FirstOrDefault(p => p.Id == reserva.PropiedadId);

            // 3. Buscar usuarios (una sola tabla con Rol)
            // 3. Buscar usuarios (una sola tabla con Rol)
            var usuarios = await _usuariosPresentacion.Listar();

            // Huésped: el usuario de la reserva
            var usuarioHuesped = usuarios.FirstOrDefault(u =>
                u.Id == reserva.UsuarioId &&
                u.Rol == RolUsuario.Huesped // o "Huesped" si Rol es string
            );

            // Anfitrión: el usuario dueño de la propiedad
            Usuarios? usuarioAnfitrion = null;
            if (propiedad != null)
            {
                usuarioAnfitrion = usuarios.FirstOrDefault(u =>
                    u.Id == propiedad.UsuarioId &&
                    u.Rol == RolUsuario.Anfitrion // o "Anfitrion"
                );
            }

            // Ajusta Nombre/Apellidos a tu entidad real
            Huesped = usuarioHuesped != null
                ? $"{usuarioHuesped.Nombre} {usuarioHuesped.Apellido}"
                : "Huésped";

            Anfitrion = usuarioAnfitrion != null
                ? $"{usuarioAnfitrion.Nombre} {usuarioAnfitrion.Apellido}"
                : "Anfitrión";

        }
    }
}
