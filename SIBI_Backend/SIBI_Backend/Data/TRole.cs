using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TRole
{
    public Guid IdRol { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public virtual ICollection<TRolesUsuario> TRolesUsuarios { get; set; } = new List<TRolesUsuario>();
}
