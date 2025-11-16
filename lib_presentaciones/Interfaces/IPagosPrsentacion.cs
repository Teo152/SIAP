using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces
{
    public interface IPagosPresentacion
    {
        Task<string> Procesar_Pago(Pagos entidad);
        Task<string> GenerarFactura(Pagos entidad);
    }
}
