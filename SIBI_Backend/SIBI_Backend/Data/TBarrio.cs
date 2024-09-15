using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TBarrio
{
    public Guid IdBarrio { get; set; }

    public string Descripcion { get; set; } = null!;

    public Guid IdCiudad { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public virtual TCiudade IdCiudadNavigation { get; set; } = null!;

    public virtual ICollection<TSocio> TSocios { get; set; } = new List<TSocio>();
}
