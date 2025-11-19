using lib_dominio.Entidades;

namespace lib_aplicaciones.Interfaces
{
    public interface IResenasAplicacion
    {
        void Configurar(string StringConexion);

     

       

        Resenas Guardar(Resenas entidad, int usuarioId);
        List<Resenas> ListarPorPropiedad(int propiedadId);
        Resenas? ObtenerPorReserva(int reservaId);
    }
}