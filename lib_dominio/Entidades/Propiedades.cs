﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text.Json.Serialization;

namespace lib_dominio.Entidades;

public class Propiedades
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public int Capacidad { get; set; }
    public string TipoPropiedad { get; set; } = null!;
    public decimal Precio { get; set; } //precio por noche

    //public bool? disponible { get; set; }
    public string EstanciaMinima { get; set; } = null!;

    public string? ReglasPropiedad { get; set; }
    public string? Descripcion { get; set; }
    public string PoliticasCancelacion { get; set; } = null!;
    public int? UsuarioId { get; set; }
    public int? MunicipioId { get; set; }
    public string? Imagen { get; set; }

    [ForeignKey("UsuarioId")][JsonIgnore] public Usuarios? _Usuarios { get; set; }
    [ForeignKey("MunicipioId")][JsonIgnore] public Municipios? _Municipios { get; set; }

    // Relación con Usuarios
    // Un anfitrión puede tener múltiples propiedades
    //  [JsonIgnore]
    //public Usuarios? Usuario { get; set; } //preguntar por ? o null!

    //[JsonIgnore] public Municipios? Municipio { get; set; }  // "

    //   [JsonIgnore] public ICollection<Reservas>? Reservas { get; set; }


}