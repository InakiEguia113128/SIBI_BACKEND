using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Usuarios;

namespace SIBI_Backend.Servicios.Usuarios
{
    public interface IServicioUsuario
    {
        Task<ResultadoBase> RegistrarUsuario(EntradaRegistrarUsuario entrada);
        Task<ResultadoBase> IniciarSesion(EntradaIniciarSesion entrada);
        Task<ResultadoBase> ObtenerUsuario(Guid id_usuario);
        Task<ResultadoBase> ModificarUsuario(EntradaModificarUsuario entrada);
    }
}