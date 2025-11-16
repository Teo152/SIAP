using lib_aplicaciones.Interfaces;
using lib_dominio.Entidades;
using lib_repositorios.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lib_aplicaciones.Implementaciones
{
    public class PropiedadesAplicacion : IPropiedadesAplicacion
    {
        private IConexion? IConexion = null;

        public PropiedadesAplicacion(IConexion iConexion)


        {
            this.IConexion = iConexion;
        }

        public void Configurar(string StringConexion)
        {
            this.IConexion!.StringConexion = StringConexion;
        }

        public Propiedades? Borrar(Propiedades? entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad!.Id == 0)
                throw new Exception("lbNoSeGuardo");

            this.IConexion!.Propiedades!.Remove(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Propiedades? Guardar(Propiedades? entidad)
        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad.Id != 0)
                throw new Exception("lbYaSeGuardo");

            if (string.IsNullOrEmpty(entidad.Imagen))
                entidad.Imagen = "sin_imagen.jpg";

         if (entidad.MunicipioId == 0 || entidad.MunicipioId.ToString() == "")
    entidad.MunicipioId = 1;

            this.IConexion!.Propiedades!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Propiedades> Listar()
        {
            return this.IConexion!.Propiedades!.Take(20).ToList();
        }

        public List<Propiedades> PorNombre(Propiedades? entidad)
        {
            return this.IConexion!.Propiedades!
                .Where(x => x.Nombre!.Contains(entidad!.Nombre!))
                .ToList();
        }

        public Propiedades? Modificar(Propiedades? entidad)


        {
            if (entidad == null)
                throw new Exception("lbFaltaInformacion");

            if (entidad!.Id == 0)
                throw new Exception("lbNoSeGuardo");



            var entry = this.IConexion!.Entry<Propiedades>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }

       
        

    }
}