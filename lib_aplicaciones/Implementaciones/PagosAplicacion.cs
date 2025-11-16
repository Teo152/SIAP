using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_aplicaciones.Implementaciones
{
    public class PagosAplicacion : IPagosAplicacion
    {
        private IConexion? IConexion = null;

        public PagosAplicacion(IConexion iConexion)


        {
            this.IConexion = iConexion;
        }

        public void Configurar(string StringConexion)
        {
            this.IConexion!.StringConexion = StringConexion;
        }

        public string Generar_factura(Pagos? entidad)
        {
            throw new NotImplementedException();
        }

       
        public string Procesar_pago(Pagos entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            // Validar número de tarjeta (16 dígitos)
            if (entidad.Numero_targeta.ToString().Length != 16)
                throw new Exception("Número de tarjeta inválido");

            // Buscar usuario
            var usuario = IConexion!.Usuarios!.FirstOrDefault(u => u.Id == entidad.UsuarioId);
            if (usuario == null)
                throw new Exception("Usuario no encontrado.");

            if (usuario.Rol != RolUsuario.Huesped)
                throw new Exception("Solo los huéspedes pueden pagar.");

            // Buscar reserva
            var reserva = IConexion!.Reservas!.FirstOrDefault(r => r.Id == entidad.ReservaId);
            if (reserva == null)
                throw new Exception("Reserva no encontrada.");

            if (reserva.EstadoId != 1)
            {
                //if (reserva.EstadoId == 2)
                //{
                    throw new Exception("Esta reserva ya fue pagada.");
                //}

               // throw new Exception("La reserva no está en estado pendiente.");


            }
                
             


            entidad.Fecha_pago = DateTime.Now;

           
            IConexion!.Pagos!.Add(entidad);
            IConexion.SaveChanges();

            reserva.EstadoId = 2;

            IConexion.Reservas.Update(reserva); // ← Forzar actualización
            IConexion.SaveChanges();


            return "Pago exitoso";
        }


    }
}