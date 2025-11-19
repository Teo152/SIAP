using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces
{
    public interface IReportesChatPresentacion
    {
        Task<ReporteChat> Crear(int reservaId, int usuarioReportanteId, string motivo);

        Task<ReporteChat?> ObtenerActivoPorReserva(int reservaId);

        Task<List<ReporteChat>> ListarActivos();

        Task<ReporteChat?> ObtenerPorId(int id);

        Task Finalizar(int id);
    }
}