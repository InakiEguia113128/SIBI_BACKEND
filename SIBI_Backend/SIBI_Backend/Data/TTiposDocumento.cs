using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TTiposDocumento
{
    public Guid IdTipoDocumento { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public virtual ICollection<TSocio> TSocios { get; set; } = new List<TSocio>();
}
