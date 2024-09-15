using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TUsuario
{
    public Guid IdUsuario { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte[] HashContraseña { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<TAlquilere> TAlquileres { get; set; } = new List<TAlquilere>();

    public virtual ICollection<TRolesUsuario> TRolesUsuarios { get; set; } = new List<TRolesUsuario>();

    public virtual TSocio? TSocio { get; set; }
}
