using SIBI_Backend.Modelos;

namespace SIBI_Backend.Servicios.Notificaciones
{
    public interface IServicioNotificaciones
    {
        Task<ResultadoBase> EnviarNotificacionBienvenida(Guid idUsuario);
    }
}
