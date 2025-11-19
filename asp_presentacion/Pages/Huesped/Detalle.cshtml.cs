using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages.Huesped
{
    public class DetalleModel : PageModel
    {
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;
        private readonly IReservasPresentacion _reservasPresentacion;
        private readonly IResenasPresentacion _resenasPresentacion;

        public DetalleModel(
            IPropiedadesPresentacion propiedadesPresentacion,
            IReservasPresentacion reservasPresentacion,
            IResenasPresentacion resenasPresentacion)
        {
            _propiedadesPresentacion = propiedadesPresentacion;
            _reservasPresentacion = reservasPresentacion;
            _resenasPresentacion = resenasPresentacion;
        }

        // ================
        // PROP. PRINCIPALES
        // ================
        public Propiedades? Propiedad { get; set; }

        // Fechas ocupadas (yyyy-MM-dd) para el calendario
        public List<string> FechasReservadas { get; set; } = new();

        // =============
        //   RESEÑAS
        // =============
        public List<Resenas> ListaResenas { get; set; } = new();
        public int TotalResenas { get; set; }
        public double PromedioCalificacion { get; set; }
        public int[] Distribucion { get; set; } = new int[5]; // [0]=?1 ... [4]=?5

        // ¿El usuario logueado puede dejar reseña en esta propiedad?
        public bool PuedeDejarResena { get; set; }
        public int? ReservaParaResenaId { get; set; }

        public async Task<IActionResult> OnGetAsync(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return RedirectToPage("./Huesped");

            // 1. Buscar la propiedad por nombre
            var lista = await _propiedadesPresentacion.PorNombre(
                new Propiedades { Nombre = nombre }
            );

            Propiedad = lista.FirstOrDefault();

            if (Propiedad == null)
                return RedirectToPage("./Huesped");

            // 2. Traer TODAS las reservas y filtrar por esta propiedad
            var todasLasReservas = await _reservasPresentacion.Listar();

            var reservasPropiedad = todasLasReservas
                .Where(r => r.PropiedadId == Propiedad.Id && r.EstadoId == 2); // 2 = pagada/confirmada (para bloquear calendario)

            var fechas = new HashSet<string>();

            foreach (var r in reservasPropiedad)
            {
                var inicio = r.Fecha_deseada.Date;
                var fin = r.Fecha_fin.Date;

                for (var f = inicio; f <= fin; f = f.AddDays(1))
                {
                    fechas.Add(f.ToString("yyyy-MM-dd"));
                }
            }

            FechasReservadas = fechas.ToList();

            // 3. Reseñas de la propiedad
            ListaResenas = await _resenasPresentacion.ListarPorPropiedad(Propiedad.Id);
            TotalResenas = ListaResenas.Count;

            if (TotalResenas > 0)
            {
                PromedioCalificacion = Math.Round(ListaResenas.Average(r => r.Calificacion), 1);

                foreach (var r in ListaResenas)
                {
                    if (r.Calificacion >= 1 && r.Calificacion <= 5)
                    {
                        // índice 0 = ?1, índice 4 = ?5
                        Distribucion[r.Calificacion - 1]++;
                    }
                }
            }

            // 4. Validar si el usuario actual puede dejar reseña
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;
            if (usuarioId > 0)
            {
                // Reservas del usuario para ESTA propiedad y en estado FINALIZADA
                // Ajusta el EstadoId según tu enum (aquí asumimos 5 = finalizada/completada)
                var reservasFinalizadas = todasLasReservas
                    .Where(r =>
                        r.UsuarioId == usuarioId &&
                        r.PropiedadId == Propiedad.Id &&
                        r.EstadoId == 5) // 5 = finalizada
                    .ToList();

                foreach (var res in reservasFinalizadas)
                {
                    bool yaTieneResena = ListaResenas.Any(x => x.ReservaId == res.Id);
                    if (!yaTieneResena)
                    {
                        PuedeDejarResena = true;
                        ReservaParaResenaId = res.Id;
                        break;
                    }
                }
            }

            return Page();
        }
    }
}
