﻿using Microsoft.EntityFrameworkCore;
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
                resultado.Resultado = usuario.TRolesUsuarios.Select(s => s.IdRolNavigation.Descripcion).ToArray();
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
    }
}