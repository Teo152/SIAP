using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_aplicaciones.Implementaciones
{
    public class ReportesChatAplicacion : IReportesChatAplicacion
    {
        private IConexion? IConexion = null;

        public ReportesChatAplicacion(IConexion conexion)
        {
            IConexion = conexion;
        }

        public void Configurar(string StringConexion)
        {
            this.IConexion!.StringConexion = StringConexion;
        }

        public ReporteChat Crear(int reservaId, int usuarioReportanteId, string motivo)
        {
            if (reservaId == 0 || usuarioReportanteId == 0)
                throw new Exception("lbFaltaInformacion");

            var reserva = this.IConexion!.Reservas!
                .Include(r => r.Propiedad)
                .FirstOrDefault(r => r.Id == reservaId)
                ?? throw new Exception("lbReservaNoEncontrada");

            var usuario = this.IConexion.Usuarios!
                .FirstOrDefault(u => u.Id == usuarioReportanteId)
                ?? throw new Exception("lbUsuarioNoEncontrado");

            var existente = this.IConexion.ReportesChat!
                .FirstOrDefault(r => r.ReservaId == reservaId && r.Activo);

            if (existente != null)
                return existente;

            var entidad = new ReporteChat
            {
                ReservaId = reservaId,
                UsuarioReportanteId = usuarioReportanteId,
                Motivo = motivo,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            this.IConexion.ReportesChat!.Add(entidad);
            this.IConexion.SaveChanges();

            return entidad;
        }

        public ReporteChat? ObtenerActivoPorReserva(int reservaId)
        {
            if (reservaId == 0)
                throw new Exception("lbFaltaInformacion");

            return this.IConexion!.ReportesChat!
                .Include(r => r.Reserva)
                    .ThenInclude(r => r.Propiedad)
                .Include(r => r.UsuarioReportante)
                .FirstOrDefault(r => r.ReservaId == reservaId && r.Activo);
        }

        public bool TieneReporteActivo(int reservaId)
        {
            return this.IConexion!.ReportesChat!
                .Any(r => r.ReservaId == reservaId && r.Activo);
        }

        public List<ReporteChat> ListarActivos()
        {
            return this.IConexion!.ReportesChat!
                .Include(r => r.Reserva)
                    .ThenInclude(r => r.Propiedad)
                .Include(r => r.UsuarioReportante)
                .Where(r => r.Activo)
                .OrderByDescending(r => r.FechaCreacion)
                .ToList();
        }

        public ReporteChat? ObtenerPorId(int id)
        {
            if (id == 0)
                throw new Exception("lbFaltaInformacion");

            return this.IConexion!.ReportesChat!
                .Include(r => r.Reserva)
                    .ThenInclude(r => r.Propiedad)
                .Include(r => r.UsuarioReportante)
                .FirstOrDefault(r => r.Id == id);
        }

        public void Finalizar(int id)
        {
            if (id == 0)
                throw new Exception("lbFaltaInformacion");

            var reporte = this.IConexion!.ReportesChat!.Find(id);
            if (reporte == null)
                throw new Exception("lbReporteNoEncontrado");

            // Primero cambio el valor
            reporte.Activo = false;

            // Y luego marco explícitamente que esa propiedad se modificó
            this.IConexion.Entry(reporte).Property(r => r.Activo).IsModified = true;

            this.IConexion.SaveChanges();
        }
    }
}