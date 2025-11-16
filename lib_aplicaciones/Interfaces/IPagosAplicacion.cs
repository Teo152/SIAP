using lib_dominio.Entidades;

using lib_dominio.Entidades;

namespace lib_aplicaciones.Interfaces
{
    public interface IPagosAplicacion
    {
        void Configurar(string StringConexion);
        string Procesar_pago(Pagos? entidad);
        string Generar_factura(Pagos? entidad);

    }
}
