using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;


namespace SIBI_Backend.Servicios.Alquileres
{
    public interface IServicioAlquiler
    {
        Task<ResultadoBase> RegistrarAlquiler (EntradaAlquiler entradaAlquiler);
    }
}
