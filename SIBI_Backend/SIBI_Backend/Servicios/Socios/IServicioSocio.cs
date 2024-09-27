using SIBI_Backend.Modelos.Socios;
using Web.Api.Softijs.Results;

namespace SIBI_Backend.Servicios.Socios
{
    public interface IServicioSocio
    {
        Task<ResultadoBase> RegistrarSocio(Guid idUsuario, EntradaRegistrarSocio entrada);
        Task<ResultadoBase> ModificarSocio(Guid idUsuario, EntradaRegistrarSocio entrada);
        Task<ResultadoBase> ObtenerSocio(Guid idUsuario);
        Task<ResultadoBase> ReactivarSocio(Guid idUsuario);
        Task<ResultadoBase> DesactivarSocio(Guid idUsuario);
        Task<ResultadoBase> ObtenerTiposDocumento();
        Task<ResultadoBase> ObtenerTiposSexo();
    }
}