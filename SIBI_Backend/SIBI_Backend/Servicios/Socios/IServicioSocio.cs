using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Socios;

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
        Task<ResultadoBase> ObtenerRankingMensual();
        Task<ResultadoBase> ObtenerPuestoSocioRankingMensual(Guid idSocio);
    }
}