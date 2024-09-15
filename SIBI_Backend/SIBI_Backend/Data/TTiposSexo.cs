using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TTiposSexo
{
    public Guid IdTipoSexo { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public virtual ICollection<TSocio> TSocios { get; set; } = new List<TSocio>();
}
