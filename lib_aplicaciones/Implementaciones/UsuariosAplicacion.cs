using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_aplicaciones.Implementaciones
{
    public class UsuariosAplicacion : IUsuariosAplicacion
    {
        private IConexion? IConexion = null;

        public UsuariosAplicacion(IConexion iConexion)


        {
            this.IConexion = iConexion;
        }

        public void Configurar(string StringConexion)
        {
            this.IConexion!.StringConexion = StringConexion;
        }

        public Usuarios? Borrar(Usuarios? entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad!.Id == 0)
                throw new Exception("lbNoSeGuardo");

            this.IConexion!.Usuarios!.Remove(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Usuarios? Guardar(Usuarios? entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id != 0)
                throw new Exception("lbYaSeGuardo");

          

            this.IConexion!.Usuarios!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Usuarios> Listar()
        {
            return this.IConexion!.Usuarios!.Take(20).ToList();
        }

        public List<Usuarios> PorEmail(Usuarios? entidad)
        {
            if (entidad == null || string.IsNullOrEmpty(entidad.Email))
                throw new Exception("El correo no puede estar vacío.");

            return this.IConexion!.Usuarios!
                .Where(x => x.Email == entidad.Email)
                .ToList();
        }


        public Usuarios? Modificar(Usuarios? entidad)


        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad!.Id == 0)
                throw new Exception("lbNoSeGuardo");

           

            var entry = this.IConexion!.Entry<Usuarios>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }
    }
}