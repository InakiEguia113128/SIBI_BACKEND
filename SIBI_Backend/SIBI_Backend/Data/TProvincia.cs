using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TProvincia
{
    public Guid IdProvincia { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public virtual ICollection<TCiudade> TCiudades { get; set; } = new List<TCiudade>();
}
