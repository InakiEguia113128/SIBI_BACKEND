using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TEstadosAlquiler
{
    public Guid IdEstadoAlquiler { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public virtual ICollection<TAlquilere> TAlquileres { get; set; } = new List<TAlquilere>();
}
