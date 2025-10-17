using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace lib_dominio.Entidades
{
    public class Reservas
    {
        public int id { get; set; }

        public DateTime fecha_creacion { get; set; }
        public DateTime fecha_deseada { get; set; }
        public DateTime fecha_fin { get; set; }

        public int cantidad_huespedes { get; set; }

        public double costo_total { get; set; }

        public int estado { get; set; }

        public int propiedad { get; set; }

        public int usuario { get; set; }

        [ForeignKey("estado")][JsonIgnore] public Estados _Estado { get; set; }
        [ForeignKey("propiedad")][JsonIgnore] public Propiedades _Propiedad { get; set; }
        [ForeignKey("usuario")][JsonIgnore] public Usuarios _Usuario { get; set; }


    }
}
