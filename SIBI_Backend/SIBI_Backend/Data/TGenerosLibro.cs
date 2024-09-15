using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TGenerosLibro
{
    public Guid IdGeneroLibro { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public virtual ICollection<TLibro> TLibros { get; set; } = new List<TLibro>();
}
