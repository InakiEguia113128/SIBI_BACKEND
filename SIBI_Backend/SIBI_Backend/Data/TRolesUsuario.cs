using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TRolesUsuario
{
    public Guid IdRolUsuario { get; set; }

    public Guid IdUsuario { get; set; }

    public Guid IdRol { get; set; }

    public virtual TRole IdRolNavigation { get; set; } = null!;

    public virtual TUsuario IdUsuarioNavigation { get; set; } = null!;
}
