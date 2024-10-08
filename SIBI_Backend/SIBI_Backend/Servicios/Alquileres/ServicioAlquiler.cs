using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Comunes;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;
using System.Linq;
using System.Xml.Schema;


namespace SIBI_Backend.Servicios.Alquileres
{
    public class ServicioAlquiler : IServicioAlquiler
    {
        private readonly SibiDbContext context;

        public ServicioAlquiler(SibiDbContext _context)
        {
            this.context = _context;
        }

        public async Task<ResultadoBase> RegistrarAlquiler(EntradaAlquiler entradaAlquiler)
        {
            var salida = new ResultadoBase();
            var lista_detalles =  new List<TDetallesAlquiler>();

            try
            {
                var socio =  await context.TSocios.FirstOrDefaultAsync(x=>x.IdUsuario == entradaAlquiler.idSocio);

                if (socio != null && entradaAlquiler.puntosCanjeados.HasValue && entradaAlquiler.puntosCanjeados > socio.PuntosAcumulados)
                {
                    salida.Ok = false;
                    salida.CodigoEstado = 400;
                    salida.Error = "El socio no posee la cantidad necesaria de puntos que desea canjear";

                    return salida;
                }

                if (socio != null && entradaAlquiler.puntosCanjeados.HasValue) 
                { 
                    socio.PuntosAcumulados = socio.PuntosAcumulados - entradaAlquiler.puntosCanjeados;
                }

                var id_alquiler = Guid.NewGuid();

                var alquiler = new TAlquilere
                {
                    IdAlquiler = id_alquiler,
                    FechaCreacion = DateOnly.FromDateTime(DateTime.Now),
                    FechaDesde = DateOnly.FromDateTime(entradaAlquiler.fechaDesde),
                    FechaHasta = DateOnly.FromDateTime(entradaAlquiler.fechaHasta),
                    IdEstadoAlquiler = EstadosAlquilerContante.Listo_para_retirar,
                    MontoTotal = entradaAlquiler.montoTotal,
                    IdSocio = entradaAlquiler.idSocio,
                    PuntosCanjeados = entradaAlquiler.puntosCanjeados
                };

                foreach (var detalle in entradaAlquiler.detalleAlquiler)
                {
                    lista_detalles.Add(new TDetallesAlquiler
                    {
                        IdDetalleAlquiler = Guid.NewGuid(),
                        IdAlquiler = id_alquiler,
                        IdLibro = detalle.idLibro,
                        PrecioAlquiler = detalle.precioAlquiler,
                        FechaCreacion = DateOnly.FromDateTime(DateTime.Now)
                    });

                    var libro = await context.TLibros.FirstOrDefaultAsync(x => x.IdLibro == detalle.idLibro);

                    libro.CantidadEjemplares = libro.CantidadEjemplares - 1;

                    if (socio.PuntosAcumulados == null)
                    {
                        socio.PuntosAcumulados = 0;
                    }
                    socio.PuntosAcumulados += 5;
                }

                alquiler.TDetallesAlquilers = lista_detalles;

                context.TAlquileres.Add(alquiler);

                await context.SaveChangesAsync();

                salida.Ok = true;
                salida.CodigoEstado = 200;
                salida.Mensaje = "Alquiler registrado con éxito!";
            }
            catch (Exception)
            {
                salida.Ok = false;
                salida.CodigoEstado = 500;
                salida.Error = "Error al intentar registrar el alquiler";
            }

            return salida;
        }

        public async Task<ResultadoBase> ObtenerAlquilerPorId(Guid id_Alquiler)
        {
            var salida = new ResultadoBase();

            try
            {
                var alquiler = await context.TAlquileres.Include(x=>x.TDetallesAlquilers).Include(x=>x.IdEstadoAlquilerNavigation).Include(x => x.IdSocioNavigation).ThenInclude(x=>x.TSocio).FirstOrDefaultAsync(x=>x.IdAlquiler == id_Alquiler);

                if(alquiler == null)
                {
                    salida.Ok = false;
                    salida.CodigoEstado = 400;
                    salida.Error = "Error al recuperar el alquiler, no fue encontrado";

                    return salida;
                }

                salida.CodigoEstado = 200;
                salida.Mensaje = "Alquiler recuperados con éxito";
                salida.Ok = true;
                salida.Resultado = new
                {
                    alquiler.IdAlquiler,
                    alquiler.IdEstadoAlquiler,
                    alquiler.IdEstadoAlquilerNavigation.Descripcion,
                    alquiler.FechaDesde,
                    alquiler.FechaHasta,
                    detallesAquiler = alquiler.TDetallesAlquilers.Select(x => new {x.IdLibro, x.FechaCreacion, x.PrecioAlquiler}),
                    alquiler.MontoTotal,
                    alquiler.PuntosCanjeados,
                    socio =  new { 
                                    SocioRegistrado = alquiler.IdSocioNavigation.TSocio == null ? false : true,
                                    alquiler.IdSocioNavigation.TSocio?.NroDocumento, 
                                    alquiler.IdSocioNavigation.TSocio?.IdTipoDocumento, 
                                    alquiler.IdSocioNavigation.TSocio?.Calle, 
                                    alquiler.IdSocioNavigation.TSocio?.Altura, 
                                    alquiler.IdSocioNavigation.TSocio?.NumeroTelefono, 
                                    alquiler.IdSocioNavigation.Nombre,
                                    alquiler.IdSocioNavigation.Apellido
                    }
                };
            }
            catch (Exception)
            {
                salida.Ok = false;
                salida.CodigoEstado = 500;
                salida.Error = "Error al intentar recuperar el alquiler";
            }

            return salida;
        }

        public async Task<ResultadoBase> ObtenerAquileres(EntradaObtenerAlquileres entrada)
        {
            var salida = new ResultadoBase();

            try
            {
                var consulta = context.TAlquileres.Include(x => x.TDetallesAlquilers).ThenInclude(x=>x.IdLibroNavigation).Include(x => x.IdEstadoAlquilerNavigation).Include(x => x.IdSocioNavigation).ThenInclude(x => x.TSocio).AsQueryable();

                if (entrada.nroDocumentoSocio.HasValue)
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.TSocio.NroDocumento == entrada.nroDocumentoSocio.Value);
                }

                if (entrada.idUsuario.HasValue)
                {
                    consulta = consulta.Where(x => x.IdSocio == entrada.idUsuario.Value);
                }

                if (entrada.idTipoDocumentoSocio.HasValue)
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.TSocio.IdTipoDocumento == entrada.idTipoDocumentoSocio);
                }

                if (!string.IsNullOrEmpty(entrada.Nombre))
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.Nombre == entrada.Nombre);
                }


                if (entrada.idEstadoAlquiler.HasValue)
                {
                    consulta = consulta.Where(x => x.IdEstadoAlquiler == entrada.idEstadoAlquiler);
                }

                if (!string.IsNullOrEmpty(entrada.Apellido))
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.Apellido == entrada.Apellido);
                }

                if (entrada.idTipoDocumentoSocio.HasValue)
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.TSocio.IdTipoDocumento == entrada.idTipoDocumentoSocio);
                }

                if (entrada.idEstadoAlquiler.HasValue)
                {
                    consulta = consulta.Where(x => x.IdEstadoAlquiler == entrada.idEstadoAlquiler.Value);
                }

                if (entrada.fechaDesde.HasValue && entrada.fechaHasta.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaDesde >= DateOnly.FromDateTime(entrada.fechaDesde.Value) && x.FechaHasta <= DateOnly.FromDateTime(entrada.fechaHasta.Value));
                }
                else if (entrada.fechaDesde.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaDesde >= DateOnly.FromDateTime(entrada.fechaDesde.Value));
                }
                else if (entrada.fechaHasta.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaHasta <= DateOnly.FromDateTime(entrada.fechaHasta.Value));
                }

                consulta = consulta.OrderByDescending(l => l.FechaCreacion);

                // Paginación
                consulta = consulta.Skip(entrada.salta);
                consulta = consulta.Take(entrada.devolver);

                var libros = await consulta.ToListAsync();

                var resultado = libros.Select(alquiler => new
                {
                    total = libros.Count,
                    alquiler.IdAlquiler,
                    alquiler.IdEstadoAlquiler,
                    alquiler.IdEstadoAlquilerNavigation.Descripcion,
                    alquiler.FechaDesde,
                    alquiler.FechaHasta,
                    detallesAlquiler = alquiler.TDetallesAlquilers.Select(x => new { x.IdLibro, x.FechaCreacion, x.PrecioAlquiler, x.IdLibroNavigation.Titulo }),
                    alquiler.MontoTotal,
                    alquiler.PuntosCanjeados,
                    socio = new
                    {
                        SocioRegistrado = alquiler.IdSocioNavigation.TSocio == null ? false : true,
                        alquiler.IdSocioNavigation.TSocio?.NroDocumento,
                        alquiler.IdSocioNavigation.TSocio?.IdTipoDocumento,
                        alquiler.IdSocioNavigation.TSocio?.Calle,
                        alquiler.IdSocioNavigation.TSocio?.Altura,
                        alquiler.IdSocioNavigation.TSocio?.NumeroTelefono,
                        alquiler.IdSocioNavigation.Nombre,
                        alquiler.IdSocioNavigation.Apellido
                    }
                });

                salida.Ok = true;
                salida.Mensaje = "Alquieleres recuperados con éxito";
                salida.Resultado = resultado;
                salida.CodigoEstado = 200;
            }
            catch (Exception)
            {
                salida.Error = "Error al obtener alquileres";
                salida.Ok = false;
                salida.CodigoEstado = 500;
            }

            return salida;
        }

        public async Task<ResultadoBase> ObtenerEstadosAlquiler()
        {
            var salida = new ResultadoBase();

            try
            {
                var estados_alquiler = await context.TEstadosAlquilers.ToListAsync();

                salida.CodigoEstado = 200;
                salida.Mensaje = "Estados de alquiler recuperados con éxito";
                salida.Ok = true;
                salida.Resultado = estados_alquiler;
            }
            catch (Exception)
            {
                salida.Ok = false;
                salida.CodigoEstado = 500;
                salida.Error = "Error al obtener los estados de alquiler";
            }

            return salida;
        }

        public async Task<ResultadoBase> CambiarEstadoAlquiler(EntradaCambiarEstadoAlquiler entrada)
        {
            var salida = new ResultadoBase();

            try
            {
                var alquiler = await context.TAlquileres.FirstOrDefaultAsync(x => x.IdAlquiler == entrada.idAlquiler);

                if (alquiler == null)
                {
                    salida.Ok = false;
                    salida.CodigoEstado = 500;
                    salida.Error = "Error al obtener alquiler";
                    return salida;
                }

                var estadoActual = alquiler.IdEstadoAlquiler;
                var nuevoEstado = entrada.idEstadoAlquiler;

                // Verifica las transiciones permitidas
                switch (estadoActual)
                {
                    case var estado when estado == EstadosAlquilerContante.Listo_para_retirar:
                        if (nuevoEstado != EstadosAlquilerContante.En_curso)
                        {
                            salida.Ok = false;
                            salida.CodigoEstado = 400;
                            salida.Error = "La transición de estado no es válida.";
                            return salida;
                        }
                        break;

                    case var estado when estado == EstadosAlquilerContante.En_curso:
                        if (nuevoEstado != EstadosAlquilerContante.Devuelto && nuevoEstado != EstadosAlquilerContante.Pendiente_devolucion)
                        {
                            salida.Ok = false;
                            salida.CodigoEstado = 400;
                            salida.Error = "La transición de estado no es válida.";
                            return salida;
                        }

                        if(nuevoEstado == EstadosAlquilerContante.Devuelto)
                        {
                            var detallesAlquiler = await context.TDetallesAlquilers.Include(x => x.IdLibroNavigation).Where(x => x.IdAlquiler == alquiler.IdAlquiler).ToListAsync();

                            foreach (var detalle in detallesAlquiler)
                            {
                                detalle.IdLibroNavigation.CantidadEjemplares = detalle.IdLibroNavigation.CantidadEjemplares + 1;
                            }
                        }
                        break;

                    case var estado when estado == EstadosAlquilerContante.Pendiente_devolucion:
                        if (nuevoEstado != EstadosAlquilerContante.Devuelto)
                        {
                            salida.Ok = false;
                            salida.CodigoEstado = 400;
                            salida.Error = "La transición de estado no es válida.";
                            return salida;
                        }
                        break;

                    case var estado when nuevoEstado == EstadosAlquilerContante.Cancelado:

                        var detalles =  await context.TDetallesAlquilers.Include(x=>x.IdLibroNavigation).Where(x=>x.IdAlquiler == alquiler.IdAlquiler).ToListAsync();

                        foreach (var detalle in detalles)
                        {
                            detalle.IdLibroNavigation.CantidadEjemplares = detalle.IdLibroNavigation.CantidadEjemplares + 1;
                        }
                        break;

                    default:
                        salida.Ok = false;
                        salida.CodigoEstado = 400;
                        salida.Error = "Estado de alquiler no reconocido.";
                        return salida;
                }

                // Cambia el estado
                alquiler.IdEstadoAlquiler = nuevoEstado;
                await context.SaveChangesAsync();

                salida.Ok = true;
                salida.CodigoEstado = 200;
                salida.Mensaje = "Estado de alquiler modificado";
            }
            catch (Exception)
            {
                salida.Ok = false;
                salida.CodigoEstado = 500;
                salida.Error = "Error al intentar cambiar el estado del alquiler.";
            }

            return salida;
        }

    }
}