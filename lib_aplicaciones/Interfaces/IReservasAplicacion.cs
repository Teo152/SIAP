using lib_dominio.Entidades;

namespace lib_aplicaciones.Interfaces
{
    public interface IReservasAplicacion
    {
        void Configurar(string StringConexion);
        //List<Reservas> PorNombre(Reservas? entidad);
        List<Reservas> Listar();
        Reservas? Guardar(Reservas? entidad);
        Reservas? Borrar(Reservas? entidad);


        //Reservas? Aprobado(Reservas? entidad);
        bool PropiedadDisponible(Reservas entidad);





       Reservas? Modificar(Reservas? entidad);
        
    }
}
