// lib_aplicaciones/Implementaciones/ResenasAplicacion.cs
using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;

namespace lib_aplicaciones.Implementaciones
{
    public class ResenasAplicacion : IResenasAplicacion
    {
        private IConexion? IConexion;

        public ResenasAplicacion(IConexion conexion)
        {
            IConexion = conexion;
        }

        public void Configurar(string stringConexion)
        {
            IConexion!.StringConexion = stringConexion;
        }

        public List<Resenas> ListarPorPropiedad(int propiedadId)
        {
            return IConexion!.Resenas!
                .Where(r => r.PropiedadId == propiedadId)
                .OrderByDescending(r => r.Fecha_creacion)
                .ToList();
        }

        public Resenas? ObtenerPorReserva(int reservaId)
        {
            return IConexion!.Resenas!
                .FirstOrDefault(r => r.ReservaId == reservaId);
        }

        public Resenas Guardar(Resenas entidad, int usuarioId)
        {
            if (entidad == null)
                throw new Exception("Falta información de la reseña.");

            if (entidad.Id != 0)
                throw new Exception("La reseña ya fue guardada.");

            if (entidad.Calificacion < 1 || entidad.Calificacion > 5)
                throw new Exception("La calificación debe estar entre 1 y 5.");

            // Buscar la reserva
            var reserva = IConexion!.Reservas!.FirstOrDefault(r => r.Id == entidad.ReservaId);
            if (reserva == null)
                throw new Exception("Reserva no encontrada.");

            // Validar que la reserva pertenezca al usuario
            if (reserva.UsuarioId != usuarioId)
                throw new Exception("Solo el huésped de la reserva puede dejar reseña.");

            // EstadoId de reserva finalizada/completada (ajusta el número según tu enum)
            if (reserva.EstadoId != 5) // 5 = Completada/Finalizada
                throw new Exception("Solo puedes calificar reservas finalizadas.");

            // Validar que no exista reseña previa para esa reserva
            if (IConexion!.Resenas!.Any(r => r.ReservaId == entidad.ReservaId))
                throw new Exception("Esta reserva ya tiene una reseña registrada.");

            entidad.PropiedadId = reserva.PropiedadId;
            entidad.Fecha_creacion = DateTime.Now;

            IConexion.Resenas!.Add(entidad);
            IConexion.SaveChanges();

            return entidad;
        }
    }
}
