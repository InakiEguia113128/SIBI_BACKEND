using MercadoPago.Resource.Preference;
using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;

namespace SIBI_Backend.Servicios.MercadoPago
{
    public interface IServicioMercadoPago
    {
        Task<ResultadoBase> CrearPreferencia(EntradaAlquiler entrada);
    }
}
