using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
using System.Net;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace SIBI_Backend.Servicios.Notificaciones
{
    public class ServicioNotificaciones : IServicioNotificaciones
    {
        private readonly SibiDbContext context;
        public ServicioNotificaciones(SibiDbContext _context)
        {
            this.context = _context;   
        }

        public async Task<ResultadoBase> EnviarNotificacionBienvenida(Guid idUsuario)
        {
            var salida = new ResultadoBase();

            try
            {
                var usuario = await context.TUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

                if (usuario == null)
                {
                    salida.Ok = false;
                    salida.CodigoEstado = 400;
                    salida.Error = "Usuario no encontrado";
                    return salida;
                }

                // Configuración de SendGrid
                var apiKey = "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";
                var client = new SendGridClient(apiKey);

                var from = new EmailAddress("sibibiblioteca@gmail.com", "SIBI");
                var to = new EmailAddress(usuario.Email, $"{usuario.Nombre} {usuario.Apellido}");

                var templateData = new
                {
                    nombre = $"{usuario.Nombre} {usuario.Apellido}"
                };


                var msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-3796c3100146420aa3b8cd6cdbe0e354", templateData);

                // Enviar el correo
                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    salida.Ok = false;
                    salida.CodigoEstado = (int)response.StatusCode;
                    salida.Error = $"Error al enviar notificación de bienvenida: {responseBody}";
                }
                else
                {
                    salida.Ok = true;
                    salida.CodigoEstado = 200;
                    salida.Mensaje = "Notificación de bienvenida enviada";
                }

                return salida;
            }
            catch (Exception ex)
            {
                salida.Ok = false;
                salida.CodigoEstado = 400;
                salida.Error = "Error al enviar notificación de bienvenida: " + ex.Message;
                return salida;
            }
        }

    }
}