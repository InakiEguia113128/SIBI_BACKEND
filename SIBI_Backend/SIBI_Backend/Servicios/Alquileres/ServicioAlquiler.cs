using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Comunes;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;


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

                if (socio == null) 
                { 
                    salida.Ok = false;
                    salida.CodigoEstado = 400;
                    salida.Error = "El socio que intenta registrar un alquiler no existe";
                    
                    return salida;
                }

                if (entradaAlquiler.puntosCanjeados.HasValue && entradaAlquiler.puntosCanjeados > socio.PuntosAcumulados)
                {
                    salida.Ok = false;
                    salida.CodigoEstado = 400;
                    salida.Error = "El socio no posee la cantidad necesaria de puntos que desea canjear";

                    return salida;
                }

                if (entradaAlquiler.puntosCanjeados.HasValue) 
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
                    IdEstadoAlquiler = EstadosAlquilerContante.Pagado,
                    MontoTotal = entradaAlquiler.montoTotal,
                    IdSocio = socio.IdUsuario,
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
                salida.Ok = true;
                salida.CodigoEstado = 500;
                salida.Error = "Error al intentar registrar el alquiler";
            }

            return salida;
        }
    }
}