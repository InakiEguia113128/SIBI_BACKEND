using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TLibro
{
    public Guid IdLibro { get; set; }

    public string CodigoIsbn { get; set; } = null!;

    public string Titulo { get; set; } = null!;

    public int CantidadEjemplares { get; set; }

    public string Descripcion { get; set; } = null!;

    public string NombreAutor { get; set; } = null!;

    public string Editorial { get; set; } = null!;

    public DateOnly FechaPublicacion { get; set; }

    public int? NroEdicion { get; set; }

    public int? NroVolumen { get; set; }

    public string CodUbicacion { get; set; } = null!;

    public Guid IdGenero { get; set; }

    public string? NGenero { get; set; }

    public int PrecioAlquiler { get; set; }

    public string ImagenPortadaBase64 { get; set; } = null!;

    public bool? Activo { get; set; }

    public DateOnly? FechaCreacion { get; set; }

    public virtual TGenerosLibro IdGeneroNavigation { get; set; } = null!;

    public virtual ICollection<TDetallesAlquiler> TDetallesAlquilers { get; set; } = new List<TDetallesAlquiler>();
}
