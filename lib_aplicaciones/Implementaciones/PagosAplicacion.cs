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
            if (entidad == null)
                throw new Exception("No se puede generar factura sin pago.");

            // Cargar relaciones si no vienen cargadas
            if (entidad.Reserva == null)
                entidad.Reserva = IConexion!.Reservas!.Find(entidad.ReservaId)!;

            if (entidad.Usuario == null)
                entidad.Usuario = IConexion!.Usuarios!.Find(entidad.UsuarioId)!;

            var reserva = entidad.Reserva;
            var usuario = entidad.Usuario;

            // Datos de la factura
            string factura = $@"

          FACTURA SIAP


Factura Nº: FAC-{entidad.Id}
Fecha de Pago: {entidad.Fecha_pago:dd/MM/yyyy}

----- DATOS DEL CLIENTE -----
Nombre: {entidad.Nombre_Apellidos}
Usuario ID: {usuario.Id}

----- DETALLES DE RESERVA -----
Reserva ID: {reserva.Id}
Check-in: {reserva.Fecha_deseada:dd/MM/yyyy}
Check-out: {reserva.Fecha_fin:dd/MM/yyyy}

----- PAGO -----
Método de pago ID: {entidad.MetodoId}
Precio Total: ${entidad.precio}

===============================
Gracias por reservar con SIAP
===============================
";

            return factura;
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

            // 🔴 ANTES: bloqueabas todo lo que no fuera estado 1
            // if (reserva.EstadoId != 1)
            // {
            //     if (reserva.EstadoId == 2)
            //         throw new Exception("Esta reserva ya fue pagada.");
            //
            //     throw new Exception("La reserva no está en estado pendiente.");
            // }

            // ✅ AHORA: permitimos pago si la reserva está en:
            // 1 = pendiente → primer pago
            // 2 = confirmada/pagada → pago de diferencia (excedente)
            if (reserva.EstadoId != 1 && reserva.EstadoId != 2)
                throw new Exception("La reserva no está en un estado válido para pagos.");

            entidad.Fecha_pago = DateTime.Now;

            IConexion!.Pagos!.Add(entidad);
            IConexion.SaveChanges();

            // Si estaba pendiente (1), al pagarla queda pagada/confirmada (2)
            if (reserva.EstadoId == 1)
            {
                reserva.EstadoId = 2;
                IConexion.Reservas.Update(reserva);
                IConexion.SaveChanges();
            }

            // Si ya estaba en 2, simplemente se registra el pago adicional
            // (excedente) y se mantiene el estado en 2

            return "Pago exitoso";
        }



    }
}