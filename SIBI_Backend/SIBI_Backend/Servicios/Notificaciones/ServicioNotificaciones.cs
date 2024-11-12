using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
using System.Net;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.DependencyInjection;
using SIBI_Backend.Comunes;

namespace SIBI_Backend.Servicios.Notificaciones
{
    public class ServicioNotificaciones : BackgroundService, IServicioNotificaciones
    {
        private readonly IServiceProvider serviceProvider; // Cambiado para usar IServiceProvider
        private readonly IConfiguration configuration;

        public ServicioNotificaciones(IServiceProvider serviceProvider, IConfiguration _configuration)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = _configuration;
        }

        public async Task<ResultadoBase> EnviarNotificacionBienvenida(Guid idUsuario)
        {
            var salida = new ResultadoBase();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SibiDbContext>();

                    var usuario = await context.TUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

                    if (usuario == null)
                    {
                        salida.Ok = false;
                        salida.CodigoEstado = 400;
                        salida.Error = "Usuario no encontrado";
                        return salida;
                    }

                    var apiKey = configuration["SendGrid:ApiKey"];
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
            }
            catch (Exception ex)
            {
                salida.Ok = false;
                salida.CodigoEstado = 400;
                salida.Error = "Error al enviar notificación de bienvenida: " + ex.Message;
                return salida;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SibiDbContext>();

                    await EnviarMailNotificacionAlquilerAVencer(context);
                    await EnviarMailNotificacionAlquilerVencido(context);
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Configuramos el tiempo que se necesita esperar
            }
        }

        private async Task EnviarMailNotificacionAlquilerVencido(SibiDbContext context)
        {
            try
            {
                var alquileres = await context.TAlquileres
                    .Where(x => x.FechaHasta < DateOnly.FromDateTime(DateTime.Now) &&
                                (x.IdEstadoAlquiler == EstadosAlquilerContante.En_curso || x.IdEstadoAlquiler == EstadosAlquilerContante.Pendiente_devolucion) &&
                                (x.FechaNotificacionVencimiento == null ||
                                x.FechaNotificacionVencimiento == DateOnly.FromDateTime(DateTime.Now)))
                    .Include(x => x.TDetallesAlquilers)
                    .ThenInclude(x => x.IdLibroNavigation)
                    .Include(x => x.IdSocioNavigation)
                    .ToListAsync();

                foreach (var alquiler in alquileres)
                {
                    var sancion = string.Empty;
                    var apiKey = configuration["SendGrid:ApiKey"];
                    var client = new SendGridClient(apiKey);

                    var from = new EmailAddress("sibibiblioteca@gmail.com", "SIBI");
                    var to = new EmailAddress(alquiler.IdSocioNavigation.Email, $"{alquiler.IdSocioNavigation.Nombre} {alquiler.IdSocioNavigation.Apellido}");

                    var usuario = await context.TSocios.FirstOrDefaultAsync(x => x.IdUsuario == alquiler.IdSocio);
                    if (usuario != null)
                    {
                        sancion = "En proximos avisos aplicaremos una sanción a tus puntos de socio";
                    }

                    if (alquiler.FechaNotificacionVencimiento != null)
                    {


                        if (usuario != null && usuario.PuntosAcumulados >= 5)
                        {
                            usuario.PuntosAcumulados = usuario.PuntosAcumulados - 5;
                            sancion = "Aplicamos una sanción de 5 puntos a tus puntos acumulados como socio";
                        }
                        if (usuario != null && usuario.PuntosAcumulados < 5 && usuario.PuntosAcumulados > 0)
                        {
                            sancion = $"Aplicamos una sanción de {usuario.PuntosAcumulados} puntos a tus puntos acumulados como socio";
                            usuario.PuntosAcumulados = 0;
                        }
                    }

                    var templateData = new
                    {
                        nombre = $"{alquiler.IdSocioNavigation.Nombre} {alquiler.IdSocioNavigation.Apellido}",
                        fecha = alquiler.FechaHasta,
                        libros = FormatearListaLibros(alquiler.TDetallesAlquilers.Select(x => x.IdLibroNavigation.Titulo)),
                        sancion
                    };

                    var msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-43713f112de64c20928cd8eecde46c46", templateData);

                    // Enviar el correo
                    var response = await client.SendEmailAsync(msg);

                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                    {
                        var responseBody = await response.Body.ReadAsStringAsync();
                    }

                    //Seteamos la fecha en 3 dias para reenviar el correo con la sancion
                    alquiler.FechaNotificacionVencimiento = DateOnly.FromDateTime(DateTime.Now.AddDays(3));
                    alquiler.IdEstadoAlquiler = EstadosAlquilerContante.Pendiente_devolucion;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task EnviarMailNotificacionAlquilerAVencer(SibiDbContext context)
        {
            try
            {
                var alquileres = await context.TAlquileres
                                   .Where(x => x.FechaHasta >= DateOnly.FromDateTime(DateTime.Now) && // Alquileres que faltan hasta 3 días
                                            x.FechaHasta <= DateOnly.FromDateTime(DateTime.Now.AddDays(3)) &&
                                            x.IdEstadoAlquiler == EstadosAlquilerContante.En_curso &&
                                            x.FechaNotificacionVencimiento == null)
                                .Include(x => x.TDetallesAlquilers)
                                .ThenInclude(x => x.IdLibroNavigation)
                                .Include(x => x.IdSocioNavigation)
                                .ToListAsync();

                foreach (var alquiler in alquileres)
                {
                    var apiKey = configuration["SendGrid:ApiKey"];
                    var client = new SendGridClient(apiKey);

                    var from = new EmailAddress("sibibiblioteca@gmail.com", "SIBI");
                    var to = new EmailAddress(alquiler.IdSocioNavigation.Email, $"{alquiler.IdSocioNavigation.Nombre} {alquiler.IdSocioNavigation.Apellido}");

                    var templateData = new
                    {
                        nombre = $"{alquiler.IdSocioNavigation.Nombre} {alquiler.IdSocioNavigation.Apellido}",
                        fecha = alquiler.FechaHasta,
                        libros = FormatearListaLibros(alquiler.TDetallesAlquilers.Select(x => x.IdLibroNavigation.Titulo))
                    };

                    var msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-26c84a31101b4ccb9138b66c6ed4e027", templateData);

                    var response = await client.SendEmailAsync(msg);

                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                    {
                        var responseBody = await response.Body.ReadAsStringAsync();
                    }

                    alquiler.FechaNotificacionProximoVencimiento = DateOnly.FromDateTime(DateTime.Now);
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private string FormatearListaLibros(IEnumerable<string> titulos)
        {
            var lista = titulos.ToList();
            if (lista.Count == 0)
            {
                return string.Empty;
            }
            else if (lista.Count == 1)
            {
                return lista[0] + ".";
            }
            else
            {
                var ultimoTitulo = lista.Last();
                lista.RemoveAt(lista.Count - 1);
                return string.Join(", ", lista) + " y " + ultimoTitulo + ".";
            }
        }

        public async Task<ResultadoBase> EnviarNotificacionDevolucionAlquiler(Guid idAlquiler)
        {
            var salida = new ResultadoBase();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SibiDbContext>();

                    var alquiler = await context.TAlquileres
                                   .Where(x => x.IdAlquiler == idAlquiler)
                                .Include(x => x.TDetallesAlquilers)
                                .ThenInclude(x => x.IdLibroNavigation)
                                .Include(x => x.IdSocioNavigation)
                                .FirstOrDefaultAsync();

                    var usuario = await context.TUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == alquiler.IdSocio);

                    if (usuario == null)
                    {
                        salida.Ok = false;
                        salida.CodigoEstado = 400;
                        salida.Error = "Usuario no encontrado";
                        return salida;
                    }

                    var apiKey = configuration["SendGrid:ApiKey"];
                    var client = new SendGridClient(apiKey);

                    var from = new EmailAddress("sibibiblioteca@gmail.com", "SIBI");
                    var to = new EmailAddress(alquiler.IdSocioNavigation.Email, $"{alquiler.IdSocioNavigation.Nombre} {alquiler.IdSocioNavigation.Apellido}");

                    var templateData = new
                    {
                        nombre = $"{alquiler.IdSocioNavigation.Nombre} {alquiler.IdSocioNavigation.Apellido}",
                        fecha = DateOnly.FromDateTime(DateTime.Now),
                        libros = FormatearListaLibros(alquiler.TDetallesAlquilers.Select(x => x.IdLibroNavigation.Titulo))
                    };

                    var msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-26a07059c4e146e3abbe2e450b8ed236", templateData);

                    var response = await client.SendEmailAsync(msg);

                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                    {
                        var responseBody = await response.Body.ReadAsStringAsync();
                        salida.Ok = false;
                        salida.CodigoEstado = (int)response.StatusCode;
                        salida.Error = $"Error al enviar notificación de devolucion: {responseBody}";
                    }
                    else
                    {
                        salida.Ok = true;
                        salida.CodigoEstado = 200;
                        salida.Mensaje = "Notificación de bienvenida devolucion";
                    }

                    return salida;
                }
            }
            catch (Exception ex)
            {
                salida.Ok = false;
                salida.CodigoEstado = 400;
                salida.Error = "Error al enviar notificación de bienvenida: " + ex.Message;
                return salida;
            }
        }

        public async Task<ResultadoBase> EnviarNotificacionDevolucionAlquilerFueraTermino(Guid idAlquiler)
        {
            var salida = new ResultadoBase();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SibiDbContext>();

                    var alquiler = await context.TAlquileres
                                   .Where(x => x.IdAlquiler == idAlquiler)
                                .Include(x => x.TDetallesAlquilers)
                                .ThenInclude(x => x.IdLibroNavigation)
                                .Include(x => x.IdSocioNavigation)
                                .FirstOrDefaultAsync();

                    var usuario = await context.TUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == alquiler.IdSocio);

                    if (usuario == null)
                    {
                        salida.Ok = false;
                        salida.CodigoEstado = 400;
                        salida.Error = "Usuario no encontrado";
                        return salida;
                    }

                    var apiKey = configuration["SendGrid:ApiKey"];
                    var client = new SendGridClient(apiKey);

                    var from = new EmailAddress("sibibiblioteca@gmail.com", "SIBI");
                    var to = new EmailAddress(alquiler.IdSocioNavigation.Email, $"{alquiler.IdSocioNavigation.Nombre} {alquiler.IdSocioNavigation.Apellido}");

                    var templateData = new
                    {
                        nombre = $"{alquiler.IdSocioNavigation.Nombre} {alquiler.IdSocioNavigation.Apellido}",
                        fecha = alquiler.FechaHasta,
                        libros = FormatearListaLibros(alquiler.TDetallesAlquilers.Select(x => x.IdLibroNavigation.Titulo)),
                        fecha_devolucion = DateOnly.FromDateTime(DateTime.Now)
                    };

                    var msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-f3f13515d1ad477888043e84194cb809", templateData);

                    var response = await client.SendEmailAsync(msg);

                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
                    {
                        var responseBody = await response.Body.ReadAsStringAsync();
                        salida.Ok = false;
                        salida.CodigoEstado = (int)response.StatusCode;
                        salida.Error = $"Error al enviar notificación de devolucion: {responseBody}";
                    }
                    else
                    {
                        salida.Ok = true;
                        salida.CodigoEstado = 200;
                        salida.Mensaje = "Notificación de bienvenida devolucion tardia";
                    }

                    return salida;
                }
            }
            catch (Exception ex)
            {
                salida.Ok = false;
                salida.CodigoEstado = 400;
                salida.Error = "Error al enviar notificación de devolucion tardia: " + ex.Message;
                return salida;
            }
        }
    }
}