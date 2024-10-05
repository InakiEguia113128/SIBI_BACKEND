using System;
using System.Collections.Generic;

namespace SIBI_Backend.Data;

public partial class TAlquilere
{
    public Guid IdAlquiler { get; set; }

    public Guid IdSocio { get; set; }

    public DateOnly FechaDesde { get; set; }

    public DateOnly FechaHasta { get; set; }

    public int MontoTotal { get; set; }

    public int? PuntosCanjeados { get; set; }

    public Guid IdEstadoAlquiler { get; set; }

    public DateOnly FechaCreacion { get; set; }

    public virtual TEstadosAlquiler IdEstadoAlquilerNavigation { get; set; } = null!;

    public virtual TUsuario IdSocioNavigation { get; set; } = null!;

    public virtual ICollection<TDetallesAlquiler> TDetallesAlquilers { get; set; } = new List<TDetallesAlquiler>();
}
