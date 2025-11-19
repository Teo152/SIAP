using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces
{
    public interface IReservasPresentacion
    {
        Task<List<Reservas>> Listar();

        Task<Reservas?> Guardar(Reservas? entidad);
        Task<Reservas?> Modificar(Reservas? entidad);

        Task<List<Reservas>> ListarParaMensajeria(int usuarioId);

        Task<Reservas?> PorId(int reservaId);
        Task<Reservas?> Borrar(Reservas? entidad);
        Task<string> ActualizarEstados();
    }
}