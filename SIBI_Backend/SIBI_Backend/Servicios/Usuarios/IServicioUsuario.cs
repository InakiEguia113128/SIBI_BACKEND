using SIBI_Backend.Modelos.Usuarios;
using Web.Api.Softijs.Results;

namespace SIBI_Backend.Servicios.Usuarios
{
    public interface IServicioUsuario
    {
        Task<ResultadoBase> RegistrarUsuario(EntradaRegistrarUsuario entrada);
    }
}