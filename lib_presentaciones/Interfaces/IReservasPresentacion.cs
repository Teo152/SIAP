using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces
{
    public interface IReservasPresentacion
    {
        Task<List<Reservas>> Listar();

        Task<Reservas?> Guardar(Reservas? entidad);

        Task<List<Reservas>> ListarParaMensajeria(int usuarioId);

        Task<Reservas?> PorId(int reservaId);
    }
}