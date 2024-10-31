using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Comunes;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;
using SIBI_Backend.Modelos.Reportes;
using System.Globalization;

namespace SIBI_Backend.Servicios.Reportes
{
    public class ServicioReportes : IServicioReportes
    {
        private readonly SibiDbContext context;

        public ServicioReportes(SibiDbContext _context)
        {
                this.context = _context;
        }

        public async Task<ResultadoBase> LibrosAlquiladosPorGenero(EntradaReporteLibrosAlquiladosPorGenero entrada)
        {
            var salida = new ResultadoBase();

            try
            {
                var consulta = context.TAlquileres
                                .Include(x => x.TDetallesAlquilers)
                                .ThenInclude(x => x.IdLibroNavigation)
                                .ThenInclude(x => x.IdGeneroNavigation)
                                .Include(x => x.IdEstadoAlquilerNavigation)
                                .AsQueryable();

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
                else
                {
                    var fechaActual = DateOnly.FromDateTime(DateTime.Now);
                    var primerDiaDelMes = new DateOnly(fechaActual.Year, fechaActual.Month, 1);

                    // Filtrar los alquileres que empiezan en este mes
                    consulta = consulta.Where(x => x.FechaDesde >= primerDiaDelMes);
                }

                var alquileres = await consulta.ToListAsync();

                var response = alquileres
                    .SelectMany(x => x.TDetallesAlquilers)
                    .GroupBy(x => new
                    {
                        x.IdLibroNavigation.IdGenero,
                        DescripcionGenero = x.IdLibroNavigation.IdGeneroNavigation.Descripcion
                    })
                    .Select(g => new
                    {
                        Genero = g.Key.DescripcionGenero,
                        CantidadAlquilados = g.Count()
                    })
                    .ToList();

                salida.Ok = true;
                salida.Mensaje = "Reporte recuperado con éxito";
                salida.Resultado = response;
                salida.CodigoEstado = 200;
            }
            catch (Exception)
            {
                salida.Error = "Error al obtener reporte";
                salida.Ok = false;
                salida.CodigoEstado = 500;
            }

            return salida;
        }

        public async Task<ResultadoBase> ObtenerAquileresVencidos(EntradaObtenerAlquileres entrada)
        {
            var salida = new ResultadoBase();

            try
            {
                var consulta = context.TAlquileres.Include(x => x.TDetallesAlquilers).ThenInclude(x => x.IdLibroNavigation).Include(x => x.IdEstadoAlquilerNavigation).Include(x => x.IdSocioNavigation).ThenInclude(x => x.TSocio).AsQueryable();

                if (entrada.nroDocumentoSocio.HasValue)
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.TSocio.NroDocumento == entrada.nroDocumentoSocio.Value);
                }

                if (entrada.idTipoDocumentoSocio.HasValue)
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.TSocio.IdTipoDocumento == entrada.idTipoDocumentoSocio);
                }

                if (!string.IsNullOrEmpty(entrada.Nombre))
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.Nombre.ToLower().Contains(entrada.Nombre.ToLower()));
                }

                if (!string.IsNullOrEmpty(entrada.Apellido))
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.Apellido.ToLower().Contains(entrada.Apellido.ToLower()));
                }

                if (entrada.idTipoDocumentoSocio.HasValue)
                {
                    consulta = consulta.Where(x => x.IdSocioNavigation.TSocio.IdTipoDocumento == entrada.idTipoDocumentoSocio);
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

                consulta = consulta.Where(x => x.IdEstadoAlquiler == EstadosAlquilerContante.Pendiente_devolucion);

                var total = await consulta.CountAsync();

                var alquileresPaginados = await consulta
                    .OrderByDescending(l => l.FechaCreacion)
                    .ToListAsync();

                var resultado = alquileresPaginados.Select(alquiler => new
                {
                    total = total,
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
                }).Skip(entrada.salta)
                   .Take(entrada.devolver);

                var resultadoPDF = alquileresPaginados.Select(alquiler => new
                {
                    total = total,
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

                var resultado2 = new
                {
                    resultado = resultado,
                    resultadoPDF = resultadoPDF
                };

                salida.Ok = true;
                salida.Mensaje = "Alquieleres recuperados con éxito";
                salida.Resultado = resultado2;
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

        public async Task<ResultadoBase> ObtenerCantidadSociosActivosPorMes(EntradaSociosActivosMes entrada)
        {
            var salida = new ResultadoBase();

            try
            {
                DateTime fechaInicio;
                DateTime fechaFin;

                // Validar fechas de entrada o usar los últimos 12 meses
                if (entrada.FechaDesde.HasValue && entrada.FechaHasta.HasValue)
                {
                    fechaInicio = entrada.FechaDesde.Value;
                    fechaFin = entrada.FechaHasta.Value;
                }
                else
                {
                    var anioActual = DateTime.Now.Year;
                    fechaInicio = new DateTime(anioActual, 1, 1);
                    fechaFin = new DateTime(anioActual, 12, 31);
                }

                var resultado = await context.TAlquileres
                    .Where(x => x.FechaCreacion >= DateOnly.FromDateTime(fechaInicio) && x.FechaCreacion <= DateOnly.FromDateTime(fechaFin))
                    .Where(x => x.IdSocioNavigation.TSocio != null)
                    .Where(x => x.IdSocioNavigation.TSocio.Activo == true|| x.IdSocioNavigation.TSocio.Activo == false)
                    .GroupBy(x => new { Mes = x.FechaCreacion.Month, Año = x.FechaCreacion.Year })
                    .Select(g => new
                    {
                        Mes = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(g.Key.Mes)),
                        g.Key.Año,
                        CantidadSociosActivos = g.Select(a => a.IdSocioNavigation.TSocio.IdUsuario).Distinct().Count()
                    })
                    .ToListAsync();

                salida.Ok = true;
                salida.Mensaje = "Socios activos recuperados con éxito";
                salida.Resultado = resultado;
                salida.CodigoEstado = 200;
            }
            catch (Exception)
            {
                salida.Error = "Error al obtener socios activos";
                salida.Ok = false;
                salida.CodigoEstado = 500;
            }

            return salida;
        }

        public async Task<ResultadoBase> ObtenerLibrosAlquilados(EntradaReporteLibrosAlquilados entrada)
        {
            var salida = new ResultadoBase();

            try
            {
                var consulta = context.TAlquileres
                    .Include(x => x.TDetallesAlquilers)
                    .ThenInclude(x => x.IdLibroNavigation)
                    .ThenInclude(x => x.IdGeneroNavigation)
                    .Include(x => x.IdEstadoAlquilerNavigation)
                    .Where(x => x.IdEstadoAlquilerNavigation.IdEstadoAlquiler != EstadosAlquilerContante.Cancelado) 
                    .AsQueryable();

                if (entrada.fechaPublicacionDesde.HasValue && entrada.fechaPublicacionHasta.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaDesde >= DateOnly.FromDateTime(entrada.fechaPublicacionDesde.Value) && x.FechaHasta <= DateOnly.FromDateTime(entrada.fechaPublicacionHasta.Value));
                }
                else if (entrada.fechaPublicacionDesde.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaDesde >= DateOnly.FromDateTime(entrada.fechaPublicacionDesde.Value));
                }
                else if (entrada.fechaPublicacionHasta.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaHasta <= DateOnly.FromDateTime(entrada.fechaPublicacionHasta.Value));
                }

                consulta = consulta.Where(x =>
                    (!string.IsNullOrEmpty(entrada.titulo) ? x.TDetallesAlquilers.Any(d => d.IdLibroNavigation.Titulo.ToLower().Contains(entrada.titulo.ToLower())) : true) &&
                    (!string.IsNullOrEmpty(entrada.autor) ? x.TDetallesAlquilers.Any(d => d.IdLibroNavigation.NombreAutor.ToLower().Contains(entrada.autor.ToLower())) : true) &&
                    (!string.IsNullOrEmpty(entrada.editorial) ? x.TDetallesAlquilers.Any(d => d.IdLibroNavigation.Editorial.ToLower().Contains(entrada.editorial.ToLower())) : true) &&
                    (!string.IsNullOrEmpty(entrada.nGenero) ? x.TDetallesAlquilers.Any(d => d.IdLibroNavigation.IdGeneroNavigation.Descripcion.ToLower().Contains(entrada.nGenero.ToLower())) : true) &&
                    (entrada.idGenero.HasValue ? x.TDetallesAlquilers.Any(d => d.IdLibroNavigation.IdGenero == entrada.idGenero) : true)
                );

                var count = await consulta
                            .SelectMany(x => x.TDetallesAlquilers)
                            .GroupBy(x => new
                            {
                                x.IdLibroNavigation.IdLibro,
                                x.IdLibroNavigation.Titulo,
                                x.IdLibroNavigation.NombreAutor,
                                Genero = x.IdLibroNavigation.IdGeneroNavigation.Descripcion,
                                x.IdLibroNavigation.Editorial,
                                OtroGenero = x.IdLibroNavigation.NGenero,
                                x.IdLibroNavigation.CantidadEjemplares,
                                x.IdLibroNavigation.FechaPublicacion
                            })
                            .CountAsync();


                var alquileres = await consulta.ToListAsync();

                var response = alquileres
                                .SelectMany(x => x.TDetallesAlquilers)
                                .GroupBy(x => x.IdLibroNavigation.IdLibro)
                                .Select(g => new
                                {
                                    Titulo = g.First().IdLibroNavigation.Titulo,
                                    NombreAutor = g.First().IdLibroNavigation.NombreAutor,
                                    Genero = g.First().IdLibroNavigation.IdGeneroNavigation.Descripcion,
                                    Editorial = g.First().IdLibroNavigation.Editorial,
                                    OtroGenero = g.First().IdLibroNavigation.NGenero,
                                    CantidadAlquilados = g.Count(),
                                    CantidadEjemplares = g.First().IdLibroNavigation.CantidadEjemplares,
                                    FechaPublicacion = g.First().IdLibroNavigation.FechaPublicacion,
                                    count // Aquí asumimos que `count` está definido previamente
                                })
                                .OrderByDescending(x => x.CantidadAlquilados)
                                .Skip(entrada.salta)
                                .Take(entrada.devolver)
                                .ToList();

                var responsePDF = alquileres
                                    .SelectMany(x => x.TDetallesAlquilers)
                                    .GroupBy(x => x.IdLibroNavigation.IdLibro)
                                    .Select(g => new
                                    {
                                        Titulo = g.First().IdLibroNavigation.Titulo,
                                        NombreAutor = g.First().IdLibroNavigation.NombreAutor,
                                        Genero = g.First().IdLibroNavigation.IdGeneroNavigation.Descripcion,
                                        Editorial = g.First().IdLibroNavigation.Editorial,
                                        OtroGenero = g.First().IdLibroNavigation.NGenero,
                                        CantidadAlquilados = g.Count(),
                                        CantidadEjemplares = g.First().IdLibroNavigation.CantidadEjemplares,
                                        FechaPublicacion = g.First().IdLibroNavigation.FechaPublicacion,
                                        count // Aquí asumimos que `count` está definido previamente
                                    })
                                    .OrderByDescending(x => x.CantidadAlquilados)
                                    .ToList();
                var resultado = new
                {
                    resultado = response,
                    resultadoPDF = responsePDF
                };

                salida.Ok = true;
                salida.Mensaje = "Reporte de libros alquilados generado con éxito";
                salida.Resultado = resultado;
                salida.CodigoEstado = 200;
            }
            catch (Exception)
            {
                salida.Error = "Error al obtener reporte de libros alquilados";
                salida.Ok = false;
                salida.CodigoEstado = 500;
            }

            return salida;
        }
    }
}