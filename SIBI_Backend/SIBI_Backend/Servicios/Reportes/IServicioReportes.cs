using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;
using SIBI_Backend.Modelos.Reportes;

namespace SIBI_Backend.Servicios.Reportes
{
    public interface IServicioReportes
    {
        Task<ResultadoBase> LibrosAlquiladosPorGenero(EntradaReporteLibrosAlquiladosPorGenero entrada);
        Task<ResultadoBase> ObtenerAquileresVencidos(EntradaObtenerAlquileres entrada);
    }
}
