using lib_dominio.Entidades;
using lib_presentaciones.Implementaciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Huesped
{
    public class EditarReservaModel : PageModel
    {
        // Reserva que se está editando (se bindea al formulario)
        [BindProperty]
        public Reservas Reserva { get; set; } = new();

        // Para mostrar info de la propiedad asociada
        public Propiedades? Propiedad { get; set; }

        // Para mostrar nombre del anfitrión si luego lo necesitas
        public string AnfitrionNombre { get; set; } = "";

        // Cantidad de noches calculadas (solo lectura)
        public int Noches =>
            (Reserva.Fecha_fin.Date - Reserva.Fecha_deseada.Date).Days;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Servicio de reservas
            var reservasServicio = new ReservasPresentacion();
            var reservas = await reservasServicio.Listar();

            Reserva = reservas.FirstOrDefault(r => r.Id == id)!;

            if (Reserva == null)
                return RedirectToPage("/Huesped/Reservas");

            // Servicio de propiedades para mostrar info en pantalla
            var propiedadesServicio = new PropiedadesPresentacion();
            var propiedades = await propiedadesServicio.Listar();
            Propiedad = propiedades.FirstOrDefault(p => p.Id == Reserva.PropiedadId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ignoramos validación de propiedades de navegación
            ModelState.Remove("Reserva.Usuario");
            ModelState.Remove("Reserva.Propiedad");

            if (!ModelState.IsValid)
                return Page();

            try
            {
                // 1. Volvemos a cargar TODAS las reservas para disponer de la original y validar solapamientos
                var reservasServicio = new ReservasPresentacion();
                var todasReservas = await reservasServicio.Listar();

                var reservaOriginal = todasReservas.FirstOrDefault(r => r.Id == Reserva.Id);
                if (reservaOriginal == null)
                {
                    ModelState.AddModelError(string.Empty, "No se encontró la reserva.");
                    return Page();
                }

                // 2. Cargar la propiedad para validar capacidad y calcular precio
                var propiedadesServicio = new PropiedadesPresentacion();
                var propiedades = await propiedadesServicio.Listar();
                Propiedad = propiedades.FirstOrDefault(p => p.Id == Reserva.PropiedadId);

                if (Propiedad == null)
                {
                    ModelState.AddModelError(string.Empty, "No se encontró la propiedad asociada.");
                    return Page();
                }

                // =========================
                // VALIDACIONES DE NEGOCIO
                // =========================

                // 1. Solo permitir modificar reservas confirmadas (ajusta EstadoId si usas otro)
                if (reservaOriginal.EstadoId != 2)
                {
                    ModelState.AddModelError(string.Empty,
                        "Solo se pueden modificar reservas confirmadas.");
                }

                // 2. Check-in debe ser posterior a hoy
                if (Reserva.Fecha_deseada.Date <= DateTime.Today)
                {
                    ModelState.AddModelError(nameof(Reserva.Fecha_deseada),
                        "La nueva fecha de check-in debe ser posterior a hoy.");
                }

                // 3. Check-out > Check-in
                if (Reserva.Fecha_fin.Date <= Reserva.Fecha_deseada.Date)
                {
                    ModelState.AddModelError(nameof(Reserva.Fecha_fin),
                        "La fecha de check-out debe ser posterior a la de check-in.");
                }

                // 4. Capacidad de huéspedes
                if (Reserva.Cantidad_huespedes < 1 ||
                    Reserva.Cantidad_huespedes > Propiedad.Capacidad)
                {
                    ModelState.AddModelError(nameof(Reserva.Cantidad_huespedes),
                        $"El máximo de huéspedes para esta propiedad es {Propiedad.Capacidad}.");
                }

                // 5. Validar disponibilidad de fechas (evitar solapamiento con otras reservas confirmadas)
                var reservasMismaPropiedad = todasReservas
                    .Where(r => r.PropiedadId == Reserva.PropiedadId
                                && r.EstadoId == 2            // solo confirmadas
                                && r.Id != Reserva.Id)        // excluir la propia
                    .ToList();

                foreach (var r in reservasMismaPropiedad)
                {
                    // Hay traslape si NO se cumple: fin <= inicioOtro  O  inicio >= finOtro
                    bool traslape = !(Reserva.Fecha_fin.Date <= r.Fecha_deseada.Date
                                      || Reserva.Fecha_deseada.Date >= r.Fecha_fin.Date);

                    if (traslape)
                    {
                        ModelState.AddModelError(string.Empty,
                            "La propiedad no está disponible en las nuevas fechas seleccionadas.");
                        break;
                    }
                }

                if (!ModelState.IsValid)
                {
                    // Volvemos a la vista con los errores
                    return Page();
                }

                // =========================
                // CALCULAR NUEVO TOTAL Y EXCEDENTE
                // =========================

                var nochesNuevas = (Reserva.Fecha_fin.Date - Reserva.Fecha_deseada.Date).Days;

                // Subtotal por noches (usa la misma lógica de CrearReserva)
                decimal subtotalNoches = nochesNuevas * Propiedad.Precio;

                // Estos valores deben coincidir con los que usaste al crear la reserva
                decimal limpieza = 50000m;   // ajusta si lo tienes configurable
                decimal impuestos = 40000m;  // ajusta si es porcentaje u otro valor

                decimal nuevoTotal = subtotalNoches + limpieza + impuestos;

                decimal totalAnterior = (decimal)reservaOriginal.Costo_total;
                decimal excedente = nuevoTotal - totalAnterior;

                // =========================
                // APLICAR CAMBIOS EN LA RESERVA
                // =========================

                reservaOriginal.Fecha_deseada = Reserva.Fecha_deseada;
                reservaOriginal.Fecha_fin = Reserva.Fecha_fin;
                reservaOriginal.Cantidad_huespedes = Reserva.Cantidad_huespedes;
                reservaOriginal.Costo_total = nuevoTotal;
                // EstadoId lo dejamos igual (confirmada)

                // Guardar cambios en la BD
                await reservasServicio.Modificar(reservaOriginal);

                // =========================
                // SI EL PRECIO SUBE ? ENVIAR A PAGO PARA COBRAR EXCEDENTE
                // =========================

                if (excedente > 0)
                {
                    // Mandamos al formulario de pago con el excedente
                    return RedirectToPage("/Huesped/Pagos", new
                    {
                        ReservaId = reservaOriginal.Id,
                        Total = excedente
                    });
                }

                // =========================
                // SI EL PRECIO ES IGUAL O MENOR ? SOLO ACTUALIZA
                // =========================

                TempData["Mensaje"] = "La reserva se actualizó correctamente.";
                return RedirectToPage("/Huesped/Reservas");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,
                    $"Error al actualizar la reserva: {ex.Message}");
                return Page();
            }
        }
    }
}
