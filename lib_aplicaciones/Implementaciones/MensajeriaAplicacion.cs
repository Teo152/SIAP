using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_aplicaciones.Implementaciones
{
    public class MensajeriaAplicacion : IMensajeriaAplicacion
    {
        private IConexion? IConexion = null;
        private readonly IReportesChatAplicacion _reportesChat;

        public MensajeriaAplicacion(IConexion iConexion, IReportesChatAplicacion reportesChat)
        {
            this.IConexion = iConexion;
            _reportesChat = reportesChat;
        }

        public void Configurar(string StringConexion)
        {
            this.IConexion!.StringConexion = StringConexion;
        }

        public Mensajes? Enviar(Mensajes? entidad, int reservaId)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.RemitenteId == 0 || entidad.DestinatarioId == 0)
                throw new Exception("lbFaltaInformacion");

            if (entidad.RemitenteId == entidad.DestinatarioId)
                throw new Exception("lbUsuariosIguales");

            if (string.IsNullOrWhiteSpace(entidad.Texto))
                throw new Exception("lbMensajeVacio");

            if (entidad.Texto.Length > 500)
                throw new Exception("lbMensajeMuyLargo");

            if (entidad.Id != 0)
                throw new Exception("lbYaSeGuardo");

            var reserva = this.IConexion!.Reservas!
                .Include(r => r.Propiedad)
                .FirstOrDefault(r => r.Id == reservaId);

            if (reserva == null)
                throw new Exception("lbReservaNoEncontrada");

            if (reserva.EstadoId != 2 && reserva.EstadoId != 3)
            {
                throw new Exception("lbEstadoReservaNoPermiteMensajeria");
            }

            var huespedId = reserva.UsuarioId;
            var anfitrionId = reserva.Propiedad?.UsuarioId ?? 0;

            if (anfitrionId == 0)
                throw new Exception("lbPropiedadSinAnfitrion");

            var esCombinacionValida =
                (entidad.RemitenteId == huespedId && entidad.DestinatarioId == anfitrionId) ||
                (entidad.RemitenteId == anfitrionId && entidad.DestinatarioId == huespedId);

            if (!esCombinacionValida)
                throw new Exception("lbUsuariosNoPertenecenALaReserva");

            entidad.ReservaId = reservaId;
            //entidad.Leido = false;

            this.IConexion!.Mensajes!.Add(entidad);
            this.IConexion.SaveChanges();

            return entidad;
        }

        public List<Mensajes> ListarConversacion(int usuario1Id, int usuario2Id, int reservaId)
        {
            if (usuario1Id == 0 || usuario2Id == 0 || reservaId == 0)
                throw new Exception("lbFaltaInformacion");

            return this.IConexion!.Mensajes!
                .Where(m =>
                    m.ReservaId == reservaId &&
                    (
                        (m.RemitenteId == usuario1Id && m.DestinatarioId == usuario2Id) ||
                        (m.RemitenteId == usuario2Id && m.DestinatarioId == usuario1Id)
                    )
                )
                .OrderBy(m => m.Id)
                .ToList();
        }

        public Mensajes? Modificar(Mensajes? entidad)
        {
            throw new Exception("lbNoSePermiteEditarMensaje");
        }

        public Mensajes? Borrar(Mensajes? entidad)
        {
            throw new Exception("lbNoSePermiteBorrarMensaje");
        }

        public int ContarNoLeidos(int usuarioDestinoId, int otroUsuarioId, int reservaId)
        {
            if (usuarioDestinoId == 0 || otroUsuarioId == 0 || reservaId == 0)
                throw new Exception("lbFaltaInformacion");

            return this.IConexion!.Mensajes!
                .Count(m =>
                    m.ReservaId == reservaId &&               // 👈 por reserva
                    m.DestinatarioId == usuarioDestinoId &&
                    m.RemitenteId == otroUsuarioId &&
                    m.Leido == false
                );
        }

        public void MarcarComoLeidos(int usuarioDestinoId, int otroUsuarioId, int reservaId)
        {
            if (usuarioDestinoId == 0 || otroUsuarioId == 0 || reservaId == 0)
                throw new Exception("lbFaltaInformacion");

            var mensajes = this.IConexion!.Mensajes!
                .Where(m =>
                    m.ReservaId == reservaId &&
                    m.DestinatarioId == usuarioDestinoId &&
                    m.RemitenteId == otroUsuarioId &&
                    m.Leido == false
                )
                .ToList();

            if (!mensajes.Any())
                return;

            foreach (var m in mensajes)
                m.Leido = true;

            this.IConexion!.Mensajes!.UpdateRange(mensajes);
            this.IConexion.SaveChanges();
        }

        public bool AdminPuedeIngresar(int reservaId)
        {
            return _reportesChat.TieneReporteActivo(reservaId);
        }

        public List<Mensajes> AdminListarConversacion(int reservaId)
        {
            if (!_reportesChat.TieneReporteActivo(reservaId))
                throw new Exception("lbChatSinReporte");

            var reserva = IConexion!.Reservas!
                .Include(r => r.Propiedad)
                .FirstOrDefault(r => r.Id == reservaId);

            if (reserva == null)
                throw new Exception("lbReservaNoEncontrada");

            var huespedId = reserva.UsuarioId;
            var anfitrionId = reserva.Propiedad?.UsuarioId ?? 0;

            if (anfitrionId == 0)
                throw new Exception("lbPropiedadSinAnfitrion");

            return IConexion!.Mensajes!
               .Where(m =>
                   m.ReservaId == reservaId &&
                   (
                       (m.RemitenteId == huespedId && m.DestinatarioId == anfitrionId) ||
                       (m.RemitenteId == anfitrionId && m.DestinatarioId == huespedId)
                   )
               )
               .OrderBy(m => m.Id)
               .ToList();
        }

        public Mensajes AdminEnviarMensaje(int reservaId, int adminId, string texto)
        {
            if (!_reportesChat.TieneReporteActivo(reservaId))
                throw new Exception("lbChatSinReporte");

            if (adminId == 0)
                throw new Exception("lbFaltaUsuario");

            if (string.IsNullOrWhiteSpace(texto))
                throw new Exception("lbMensajeVacio");

            var reserva = IConexion!.Reservas!
                .Include(r => r.Propiedad)
                .FirstOrDefault(r => r.Id == reservaId);

            if (reserva == null)
                throw new Exception("lbReservaNoEncontrada");

            var huespedId = reserva.UsuarioId;
            var anfitrionId = reserva.Propiedad?.UsuarioId ?? 0;

            if (anfitrionId == 0)
                throw new Exception("lbPropiedadSinAnfitrion");

            var mensaje = new Mensajes
            {
                Texto = "[ADMIN] " + texto,
                RemitenteId = anfitrionId,
                DestinatarioId = huespedId,
                ReservaId = reservaId,
                Leido = false,
                EsAdmin = true
            };

            IConexion.Mensajes!.Add(mensaje);
            IConexion.SaveChanges();

            return mensaje;
        }
    }
}