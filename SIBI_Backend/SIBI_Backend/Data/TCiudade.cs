using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TCiudade
{
    public Guid IdCiudad { get; set; }

    public string Descripcion { get; set; } = null!;

    public Guid IdProvincia { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public virtual TProvincia IdProvinciaNavigation { get; set; } = null!;

    public virtual ICollection<TBarrio> TBarrios { get; set; } = new List<TBarrio>();
}
