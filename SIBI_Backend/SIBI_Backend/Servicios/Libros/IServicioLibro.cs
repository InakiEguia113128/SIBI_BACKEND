using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Libros;


namespace SIBI_Backend.Servicios.Libros
{
    public interface IServicioLibro
    {
        Task<ResultadoBase> RegistrarLibro(EntradaRegistrarLibro entrada);
        Task<ResultadoBase> ObtenerGeneros();
        Task<ResultadoBase> ObtenerCatalogo(EntradaObtenerCatalogo entrada);
        Task<ResultadoBase> ElminarLibro(Guid idLibro);
        Task<ResultadoBase> ModificarLibro(Guid idLibro, EntradaModificarLibro entrada);
        Task<ResultadoBase> ObtenerLibroPorId(Guid idLibro);
        Task<ResultadoBase> ObtenerLibroISBN(string ISBN);
    }
}
