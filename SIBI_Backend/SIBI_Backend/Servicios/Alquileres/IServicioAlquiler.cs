using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;


namespace SIBI_Backend.Servicios.Alquileres
{
    public interface IServicioAlquiler
    {
        Task<ResultadoBase> RegistrarAlquiler (EntradaAlquiler entradaAlquiler);
        Task<ResultadoBase> ObtenerAquileres(EntradaObtenerAlquileres entradaAlquiler);
        Task<ResultadoBase> ObtenerAlquilerPorId(Guid id_Alquiler);
        Task<ResultadoBase> ObtenerEstadosAlquiler();
        Task<ResultadoBase> CambiarEstadoAlquiler(EntradaCambiarEstadoAlquiler entradaAlquiler);
    }
}
