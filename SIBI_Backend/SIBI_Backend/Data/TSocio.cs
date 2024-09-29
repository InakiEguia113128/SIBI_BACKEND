using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TSocio
{
    public Guid IdUsuario { get; set; }

    public DateOnly FechaNacimiento { get; set; }

    public long NumeroTelefono { get; set; }

    public Guid? IdBarrio { get; set; }

    public string Calle { get; set; } = null!;

    public int Altura { get; set; }

    public Guid IdSexo { get; set; }

    public Guid IdTipoDocumento { get; set; }

    public long NroDocumento { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public int? PuntosAcumulados { get; set; }

    public bool Activo { get; set; }

    public virtual TBarrio? IdBarrioNavigation { get; set; }

    public virtual TTiposSexo IdSexoNavigation { get; set; } = null!;

    public virtual TTiposDocumento IdTipoDocumentoNavigation { get; set; } = null!;

    public virtual TUsuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<TAlquilere> TAlquileres { get; set; } = new List<TAlquilere>();
}
