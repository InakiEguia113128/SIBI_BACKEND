using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
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
    }
}