using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Comunes;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;
using SIBI_Backend.Modelos.Reportes;

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
                    .Skip(entrada.salta)
                    .Take(entrada.devolver)
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
    }
}