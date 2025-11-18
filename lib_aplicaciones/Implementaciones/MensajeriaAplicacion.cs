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
            // ===== VALIDACIONES BÁSICAS =====
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

            // ===== VALIDAR RESERVA =====
            // Se carga la reserva con su propiedad para identificar huésped y anfitrión
            var reserva = this.IConexion!.Reservas!
                .Include(r => r.Propiedad)
                .FirstOrDefault(r => r.Id == reservaId);

            if (reserva == null)
                throw new Exception("lbReservaNoEncontrada");

            // Estados permitidos para mensajería: Activo o Aprovado (confirmada)
            if (reserva.EstadoId != 2 && reserva.EstadoId != 3)
            {
                throw new Exception("lbEstadoReservaNoPermiteMensajeria");
            }

            // Identificamos huésped y anfitrión
            var huespedId = reserva.UsuarioId;                    // quien hizo la reserva
            var anfitrionId = reserva.Propiedad?.UsuarioId ?? 0;  // dueño de la propiedad

            if (anfitrionId == 0)
                throw new Exception("lbPropiedadSinAnfitrion");

            // Validar que el mensaje sea entre huésped y anfitrión de esta reserva
            var esCombinacionValida =
                (entidad.RemitenteId == huespedId && entidad.DestinatarioId == anfitrionId) ||
                (entidad.RemitenteId == anfitrionId && entidad.DestinatarioId == huespedId);

            if (!esCombinacionValida)
                throw new Exception("lbUsuariosNoPertenecenALaReserva");

            // ===== GUARDAR MENSAJE =====
            this.IConexion!.Mensajes!.Add(entidad);
            this.IConexion.SaveChanges();

            return entidad;
        }

        public List<Mensajes> ListarConversacion(int usuario1Id, int usuario2Id)
        {
            if (usuario1Id == 0 || usuario2Id == 0)
                throw new Exception("lbFaltaInformacion");

            return this.IConexion!.Mensajes!
                .Where(m =>
                    (m.RemitenteId == usuario1Id && m.DestinatarioId == usuario2Id) ||
                    (m.RemitenteId == usuario2Id && m.DestinatarioId == usuario1Id)
                )
                // Suponemos que el Id crece con el tiempo;
                // si luego agregas FechaHora al mensaje, se cambia a OrderBy(m => m.FechaHora)
                .OrderBy(m => m.Id)
                .ToList();
        }

        // ===== RESTRICCIÓN: NO SE PERMITE EDITAR NI BORRAR =====
        public Mensajes? Modificar(Mensajes? entidad)
        {
            // CA-4: no se puede editar el mensaje.
            throw new Exception("lbNoSePermiteEditarMensaje");
        }

        public Mensajes? Borrar(Mensajes? entidad)
        {
            // CA-4: no se puede eliminar el mensaje.
            throw new Exception("lbNoSePermiteBorrarMensaje");
        }

        public int ContarNoLeidos(int usuarioDestinoId, int otroUsuarioId)
        {
            if (usuarioDestinoId == 0 || otroUsuarioId == 0)
                throw new Exception("lbFaltaInformacion");

            return this.IConexion!.Mensajes!
                .Count(m =>
                    m.DestinatarioId == usuarioDestinoId &&   // 👈 YO soy el destino
                    m.RemitenteId == otroUsuarioId &&       // 👈 él es el otro
                    m.Leido == false
                );
        }

        public void MarcarComoLeidos(int usuarioDestinoId, int otroUsuarioId)
        {
            if (usuarioDestinoId == 0 || otroUsuarioId == 0)
                throw new Exception("lbFaltaInformacion");

            // Traemos los mensajes no leídos
            var mensajes = this.IConexion!.Mensajes!
                .Where(m =>
                    m.DestinatarioId == usuarioDestinoId &&
                    m.RemitenteId == otroUsuarioId &&
                    m.Leido == false
                )
                .ToList();

            if (!mensajes.Any())
                return;

            // Marcamos como leídos
            foreach (var m in mensajes)
                m.Leido = true;

            // 👇 FORZAMOS A EF A MARCARLOS COMO MODIFICADOS
            this.IConexion!.Mensajes!.UpdateRange(mensajes);

            // Guardamos cambios
            this.IConexion.SaveChanges();
        }

        public bool AdminPuedeIngresar(int reservaId)
        {
            return _reportesChat.TieneReporteActivo(reservaId);
        }

        /// <summary>
        /// Devuelve la conversación huésped-anfitrión para una reserva,
        /// solo si existe reporte activo.
        /// </summary>
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

            return ListarConversacion(huespedId, anfitrionId);
        }

        /// <summary>
        /// Enviar mensaje como "Administrador - Soporte" en el chat de una reserva.
        /// </summary>
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

            // Decisión: mandamos el mensaje a los dos, pero en BD
            // lo guardamos con DestinatarioId = 0 (broadcast lógico)
            // o si prefieres, al huésped; aquí lo dejo dirigido al huésped.
            var mensaje = new Mensajes
            {
                Texto = texto,
                RemitenteId = adminId,
                DestinatarioId = huespedId, // puedes cambiar lógica si quieres
                Leido = false,
                EsAdmin = true
            };

            IConexion.Mensajes!.Add(mensaje);
            IConexion.SaveChanges();

            return mensaje;
        }
    }
}