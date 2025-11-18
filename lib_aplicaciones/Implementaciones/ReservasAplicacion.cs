using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_aplicaciones.Implementaciones
{
    public class ReservasAplicacion : IReservasAplicacion
    {
        private IConexion? IConexion = null;

        public ReservasAplicacion(IConexion iConexion)

        {
            this.IConexion = iConexion;
        }

        public void Configurar(string StringConexion)
        {
            this.IConexion!.StringConexion = StringConexion;
        }

        /* public Reservas? Borrar(Reservas? entidad)
         {
             if (entidad == null)
                 throw new Exception("lbFaltaInformacion");

             if (entidad!.Id == 0)
                 throw new Exception("lbNoSeGuardo");

             this.IConexion!.Reservas!.Remove(entidad);
             this.IConexion.SaveChanges();
             return entidad;
         }*/

        //Este metodo genera una nueva reserva
        public Reservas Guardar(Reservas entidad)
        {
            if (entidad == null)
                throw new Exception("Falta información de la reserva.");

            if (entidad.Id != 0)
                throw new Exception("La reserva ya fue guardada.");

            var usuario = this.IConexion!.Usuarios!.FirstOrDefault(u => u.Id == entidad.UsuarioId);
            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            if (usuario.Rol != RolUsuario.Huesped)
                throw new Exception("Solo los usuarios con rol Huesped pueden realizar reservas.");

            if (!PropiedadDisponible(entidad))
                throw new Exception("La propiedad no está disponible en las fechas seleccionadas.");

            entidad.EstadoId = 1;
            entidad.Fecha_creacion = DateTime.Now;

            this.IConexion!.Reservas!.Add(entidad);
            this.IConexion.SaveChanges();

            return entidad;
        }

        public List<Reservas> Listar()
        {
            return this.IConexion!.Reservas!.Take(20).ToList();
        }

        /* public List<Reservas> PorNombre(Reservas? entidad)
         {
             return this.IConexion!.Reservas!
                 .Where(x => x.Nombre!.Contains(entidad!.Nombre!))
                 .ToList();
         }/*

         /*   public Reservas? Modificar(Reservas? entidad)

            {
                if (entidad == null)
                    throw new Exception("lbFaltaInformacion");

                if (entidad!.Id == 0)
                    throw new Exception("lbNoSeGuardo");

                var entry = this.IConexion!.Entry<Reservas>(entidad);
                entry.State = EntityState.Modified;
                this.IConexion.SaveChanges();
                return entidad;
                }*/

        public bool PropiedadDisponible(Reservas entidad)
        {
            return !this.IConexion!.Reservas!
                .Any(r =>
                    r.PropiedadId == entidad.PropiedadId &&
                    (r.EstadoId == 1 || r.EstadoId == 2) && // Pendiente o Aprobada
                    (
                        (entidad.Fecha_deseada >= r.Fecha_deseada && entidad.Fecha_deseada < r.Fecha_fin) ||
                        (entidad.Fecha_fin > r.Fecha_deseada && entidad.Fecha_fin <= r.Fecha_fin) ||
                        (entidad.Fecha_deseada <= r.Fecha_deseada && entidad.Fecha_fin >= r.Fecha_fin)
                    )
                );
        }

        public List<Reservas> ListarParaMensajeria(int usuarioId)
        {
            if (usuarioId == 0)
                throw new Exception("lbFaltaInformacion");

            int estadoAprovado = (int)Estados.Aprovado;
            int estadoActivo = (int)Estados.Activo;

            var query = this.IConexion!.Reservas!
                .Include(r => r.Propiedad)
                .Include(r => r.Usuario)
                .Where(r =>
                    (r.EstadoId == estadoAprovado || r.EstadoId == estadoActivo) &&
                    (
                        // Soy huésped
                        r.UsuarioId == usuarioId
                        ||
                        // Soy anfitrión (dueño de la propiedad)
                        (r.Propiedad.UsuarioId.HasValue && r.Propiedad.UsuarioId.Value == usuarioId)
                    )
                );

            return query.ToList();
        }

        public Reservas? PorId(int reservaId)
        {
            if (reservaId == 0)
                throw new Exception("lbFaltaReservaId");

            return this.IConexion!.Reservas!
                .Include(r => r.Propiedad)
                .FirstOrDefault(r => r.Id == reservaId);
        }
    }
}