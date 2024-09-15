using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TUsuario
{
    public Guid IdUsuario { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashContraseña { get; set; } = null!;

    public DateOnly FechaCreacion { get; set; }

    public byte[] Activo { get; set; } = null!;

    public virtual ICollection<TAlquilere> TAlquileres { get; set; } = new List<TAlquilere>();

    public virtual ICollection<TRolesUsuario> TRolesUsuarios { get; set; } = new List<TRolesUsuario>();

    public virtual TSocio? TSocio { get; set; }
}
