// lib_presentaciones/Interfaces/IResenasPresentacion.cs
using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces
{
    public interface IResenasPresentacion
    {
        Task<List<Resenas>> ListarPorPropiedad(int propiedadId);
        Task<Resenas?> ObtenerPorReserva(int reservaId);
        Task<Resenas?> Guardar(Resenas entidad, int usuarioId);
    }
}
