using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SIBI_Backend.Comunes;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Socios;

namespace SIBI_Backend.Servicios.Socios
{
    public class ServicioSocio : IServicioSocio
    {
        private readonly SibiDbContext context;
        public ServicioSocio(SibiDbContext _context)
        {
            this.context = _context;
        }

        public async Task<ResultadoBase> RegistrarSocio(Guid idUsuario, EntradaRegistrarSocio entrada)
        {
            var resultado = new ResultadoBase();

            try
            {
                var usuario = await context.TUsuarios.Include(x => x.TRolesUsuarios).ThenInclude(x => x.IdRolNavigation).FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

                if (usuario == null)
                {
                    resultado.Error = "Usuario no encontrado";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                context.TSocios.Add(new TSocio 
                {
                    IdUsuario = usuario.IdUsuario,
                    FechaNacimiento = DateOnly.FromDateTime(entrada.fechaNacimiento),
                    FechaCreacion = DateOnly.FromDateTime(DateTime.Now),
                    Calle = entrada.calle,
                    Altura = entrada.altura,
                    IdSexo = entrada.idSexo,
                    IdTipoDocumento = entrada.idTipoDocumento,
                    NroDocumento = entrada.nroDocumento,
                    NumeroTelefono = entrada.numeroTelefono,
                    Activo  = true
                });

                usuario.TRolesUsuarios.Add(new TRolesUsuario
                {
                    IdRolUsuario = Guid.NewGuid(),
                    IdRol = RolesConstante.SocioRegistrado,
                    IdUsuario = usuario.IdUsuario
                });

                await context.SaveChangesAsync();

                resultado.Mensaje = "Socio registrado con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
            }
            catch (Exception)
            {
                resultado.Error = "Error al registrar socio";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> ModificarSocio(Guid idUsuario, EntradaRegistrarSocio entrada)
        {
            var resultado = new ResultadoBase();

            try
            {
                var socio = await context.TSocios.FirstOrDefaultAsync(x=>x.IdUsuario == idUsuario);

                if(socio == null)
                {
                    resultado.Error = "El socio que intenta modificar no existe";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                socio.FechaNacimiento = DateOnly.FromDateTime(entrada.fechaNacimiento);
                socio.Calle = entrada.calle;
                socio.Altura = entrada.altura;
                socio.IdSexo = entrada.idSexo;
                socio.IdTipoDocumento = entrada.idTipoDocumento;
                socio.NroDocumento = entrada.nroDocumento;
                socio.NumeroTelefono = entrada.numeroTelefono;

                await context.SaveChangesAsync();

                resultado.Mensaje = "Socio modificado con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
            }
            catch (Exception)
            {
                resultado.Error = "Error al modificar socio";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> ObtenerSocio(Guid idUsuario)
        {
            var resultado = new ResultadoBase();

            try
            {
                var socio = await context.TSocios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

                if (socio == null)
                {
                    resultado.Error = "El socio que intenta recuperar no existe";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                resultado.Mensaje = "Socio recuperado con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
                resultado.Resultado = socio;
            }
            catch (Exception)
            {
                resultado.Error = "Error al recuperar socio";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> ReactivarSocio(Guid idUsuario)
        {
            var resultado = new ResultadoBase();

            try
            {
                var socio = await context.TSocios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

                if (socio == null)
                {
                    resultado.Error = "El socio que intenta reactivar no existe";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                socio.Activo = true;

                await context.SaveChangesAsync();

                resultado.Mensaje = "Socio reactivado con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
                resultado.Resultado = socio;
            }
            catch (Exception)
            {
                resultado.Error = "Error al reactivar socio";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> DesactivarSocio(Guid idUsuario)
        {
            var resultado = new ResultadoBase();

            try
            {
                var socio = await context.TSocios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);

                if (socio == null)
                {
                    resultado.Error = "El socio que intenta desactivar no existe";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                socio.Activo = false;

                await context.SaveChangesAsync();

                resultado.Mensaje = "Socio desactivado con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
                resultado.Resultado = socio;
            }
            catch (Exception)
            {
                resultado.Error = "Error al desactivar socio";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> ObtenerTiposDocumento()
        {
            var resultado = new ResultadoBase();

            try
            {
                var tipos_documento = await context.TTiposDocumentos.ToListAsync();

                resultado.Mensaje = "Tipos de documento recuperados con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
                resultado.Resultado = tipos_documento;
            }
            catch (Exception)
            {
                resultado.Error = "Error al obtener los tipos de documento";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> ObtenerTiposSexo()
        {
            var resultado = new ResultadoBase();

            try
            {
                var tipos_documento = await context.TTiposSexos.ToListAsync();

                resultado.Mensaje = "Tipos de sexo recuperados con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
                resultado.Resultado = tipos_documento;
            }
            catch (Exception)
            {
                resultado.Error = "Error al obtener los tipos de sexo";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }


            return resultado;
        }

        public async Task<ResultadoBase> ObtenerRankingMensual()
        {
            var resultado = new ResultadoBase();

            try
            {
                var socios = await context.TSocios
                            .Include(s => s.IdUsuarioNavigation)
                                .ThenInclude(u => u.TAlquileres)
                                    .ThenInclude(a => a.TDetallesAlquilers)
                            .Where(s => s.Activo &&
                                        s.IdUsuarioNavigation.TAlquileres
                                            .Any(a => a.FechaDesde.Month == DateTime.Now.Month && a.FechaDesde.Year == DateTime.Now.Year))
                            .ToListAsync();


                var ranking = socios
                            .Select(socio => new
                            {
                                socio.NroDocumento,
                                socio.IdTipoDocumento,
                                socio.Calle,
                                socio.Altura,
                                socio.NumeroTelefono,
                                socio.IdUsuarioNavigation.Nombre,
                                socio.IdUsuarioNavigation.Apellido,
                                CantidadLibrosAlquilados = socio.IdUsuarioNavigation.TAlquileres
                                    .Where(x=>x.IdEstadoAlquiler != EstadosAlquilerContante.Listo_para_retirar && x.IdEstadoAlquiler != EstadosAlquilerContante.Cancelado)
                                    .SelectMany(alquiler => alquiler.TDetallesAlquilers)
                                    .Count()
                            })
                            .Where(s => s.CantidadLibrosAlquilados > 0)
                            .OrderByDescending(s => s.CantidadLibrosAlquilados) 
                            .Take(10)
                            .ToList();

                resultado.Ok = true;
                resultado.Mensaje = "Alquieleres recuperados con éxito";
                resultado.Resultado = ranking;
                resultado.CodigoEstado = 200;
            }
            catch (Exception)
            {
                resultado.Error = "Error al obtener ranking mensual";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> ObtenerPuestoSocioRankingMensual(Guid idSocio)
        {
            var resultado = new ResultadoBase();

            try
            {

                var socios = await context.TSocios
                    .Include(s => s.IdUsuarioNavigation)
                        .ThenInclude(u => u.TAlquileres)
                            .ThenInclude(a => a.TDetallesAlquilers)
                    .Where(s => s.Activo &&
                                s.IdUsuarioNavigation.TAlquileres
                                    .Any(a => a.FechaDesde.Month == DateTime.Now.Month && a.FechaDesde.Year == DateTime.Now.Year))
                    .ToListAsync();

                var ranking = socios.Select(socio => new
                {
                    socio.NroDocumento,
                    socio.IdTipoDocumento,
                    socio.Calle,
                    socio.Altura,
                    socio.NumeroTelefono,
                    socio.IdUsuarioNavigation.Nombre,
                    socio.IdUsuarioNavigation.Apellido,
                    socio.IdUsuarioNavigation.IdUsuario,
                    CantidadLibrosAlquilados = socio.IdUsuarioNavigation.TAlquileres
                        .SelectMany(alquiler => alquiler.TDetallesAlquilers)
                        .Count()
                })
                .OrderByDescending(s => s.CantidadLibrosAlquilados) 
                .ToList();

                var socioEspecifico = ranking
                    .Select((s, index) => new
                    {
                        Posicion = index + 1, 
                        Socio = s
                    })
                    .FirstOrDefault(s => s.Socio.IdUsuario == idSocio);

                if(socioEspecifico == null)
                {
                    resultado.Error = "Error al obtener posicion de socio en ranking mensual, el socio no fue encontrado";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }


                resultado.Ok = true;
                resultado.Mensaje = "Posición del socio en el ranking mensual recuperada";
                resultado.Resultado = new 
                {
                    Posicion = socioEspecifico?.Posicion ?? 0,
                    Socio = socioEspecifico.Socio
                };
                resultado.CodigoEstado = 200;

            }
            catch (Exception)
            {
                resultado.Error = "Error al obtener posicion de socio en ranking mensual";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }
    }
}