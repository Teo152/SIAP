using lib_dominio.Entidades;

namespace lib_aplicaciones.Interfaces
{
    public interface IReportesChatAplicacion
    {
        void Configurar(string StringConexion);

        ReporteChat Crear(int reservaId, int usuarioReportanteId, string motivo);

        ReporteChat? ObtenerActivoPorReserva(int reservaId);

        List<ReporteChat> ListarActivos();

        ReporteChat? ObtenerPorId(int id);

        bool TieneReporteActivo(int reservaId);

        void Finalizar(int id);
    }
}