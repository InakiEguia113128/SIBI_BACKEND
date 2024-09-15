using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TDetallesAlquiler
{
    public Guid IdDetalleAlquiler { get; set; }

    public Guid IdAlquiler { get; set; }

    public Guid IdLibro { get; set; }

    public int PrecioAlquiler { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public virtual TAlquilere IdAlquilerNavigation { get; set; } = null!;

    public virtual TLibro IdLibroNavigation { get; set; } = null!;
}
