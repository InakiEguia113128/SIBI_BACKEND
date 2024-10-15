using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Comunes;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos.Usuarios;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SIBI_Backend.Modelos;
using SIBI_Backend.Servicios.Notificaciones;

namespace SIBI_Backend.Servicios.Usuarios
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly SibiDbContext context;
        private readonly IConfiguration config;
        private readonly IServicioNotificaciones nofiticaciones;
        public ServicioUsuario(SibiDbContext _context, IConfiguration _config, IServicioNotificaciones _notificaciones)
        {
            this.context = _context;
            this.config = _config;
            this.nofiticaciones = _notificaciones;
        }

        public async Task<ResultadoBase> RegistrarUsuario(EntradaRegistrarUsuario entrada)
        {
            var resultado = new ResultadoBase();
            try
            {
                if(await context.TUsuarios.AnyAsync(x=> x.Email == entrada.email))
                {
                    resultado.Error = "El email ingresado ya se encuentra registrado";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }
                if (!validarEmail(entrada.email)) 
                {
                    resultado.Error = "El email ingresado posee un formato incorrecto";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                byte[] ePass = GetHash(entrada.contrasenia);

                var idUsuario = Guid.NewGuid();

                await context.TUsuarios.AddAsync(new TUsuario()
                {
                    IdUsuario = idUsuario,
                    Nombre = entrada.nombre,
                    Apellido = entrada.apellido,
                    Email = entrada.email,
                    HashContraseña = ePass,
                    FechaCreacion = DateOnly.FromDateTime(DateTime.Now),
                    Activo = true,
                    TRolesUsuarios = new List<TRolesUsuario>
                    {
                        new TRolesUsuario
                        {
                            IdRolUsuario = Guid.NewGuid(),
                            IdRol = RolesConstante.Socio,
                            IdUsuario = idUsuario
                        }
                    }
                });

                await context.SaveChangesAsync();

                await nofiticaciones.EnviarNotificacionBienvenida(idUsuario);

                resultado.Mensaje = "Usuario registrado con exito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
            }
            catch (Exception)
            {
                resultado.Error = "Error al registrar usuario";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> IniciarSesion(EntradaIniciarSesion entrada)
        {
            var respuesta = new ResultadoBase();

            try
            {
                var ePass = GetHash(entrada.contrasenia);

                var usuario = await context.TUsuarios
                    .Include(x => x.TRolesUsuarios)
                    .ThenInclude(x => x.IdRolNavigation)
                    .FirstOrDefaultAsync(c => c.Email == entrada.email && c.HashContraseña == ePass);

                if(usuario == null)
                {
                    respuesta.Ok = false;
                    respuesta.CodigoEstado = 400;
                    respuesta.Error = "El email o contraseña incorrecto";

                    return respuesta;
                }

                if ((bool)!usuario.Activo)
                {
                    respuesta.Ok = false;
                    respuesta.CodigoEstado = 400;
                    respuesta.Error = "El usuario se encuentra inactivo";

                    return respuesta;
                }

                var claims = new[]
                {
                     new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                     new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                     new Claim(ClaimTypes.Email, usuario.Email),
                     new Claim(ClaimTypes.Role, string.Join(",", usuario.TRolesUsuarios.Select(s => s.IdRolNavigation.Descripcion).ToArray()))
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(double.Parse(config.GetSection("AppSettings:Expires").Value)),
                    SigningCredentials = creds,
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                var resultado = new
                {
                    usuario = new
                    {
                        IdUsuario = usuario.IdUsuario,
                        Activo = usuario.Activo,
                        Email = usuario.Email,
                        Nombre = $"{usuario.Nombre} {usuario.Apellido}",
                        Roles = usuario.TRolesUsuarios.Select(s => s.IdRolNavigation.Descripcion).ToArray()
                    },
                    Token = tokenHandler.WriteToken(token)
                };

                respuesta.Mensaje = "Sesión iniciada exitosamente";
                respuesta.Ok = true;
                respuesta.CodigoEstado = 200;
                respuesta.Resultado = resultado;
            }
            catch (Exception)
            {
                respuesta.Ok = false;
                respuesta.CodigoEstado = 400;
                respuesta.Error = "Error al iniciar sesión";
            }

            return respuesta;
        }

        public async Task<ResultadoBase> ModificarUsuario(EntradaModificarUsuario entrada)
        {
            var resultado = new ResultadoBase();

            try
            {
                if (await context.TUsuarios.AnyAsync(x => x.Email == entrada.email && x.IdUsuario != entrada.idUsuario))
                {
                    resultado.Error = "El email ingresado ya se encuentra registrado";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }
                if (!validarEmail(entrada.email))
                {
                    resultado.Error = "El email ingresado posee un formato incorrecto";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                var usuario = await context.TUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == entrada.idUsuario);

                if(usuario == null)
                {
                    resultado.Error = "El usuario no fue encontrado";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                usuario.Nombre = entrada.nombre;
                usuario.Apellido = entrada.apellido;
                usuario.Email = entrada.email;

                if(!string.IsNullOrWhiteSpace(entrada.contrasenia))
                {
                    usuario.HashContraseña = GetHash(entrada.contrasenia);
                }

                await context.SaveChangesAsync();

                resultado.Mensaje = "Usuario modificado exitosamente";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
            }
            catch (Exception)
            {
                resultado.Ok = false;
                resultado.CodigoEstado = 400;
                resultado.Error = "Error al iniciar modificar usuario";
            }

            return resultado;
        }

        public async Task<ResultadoBase> ObtenerUsuario(Guid id_usuario)
        {
            var resultado = new ResultadoBase();

            try
            {
                var usuario = await context.TUsuarios.Include(x => x.TRolesUsuarios)
                    .ThenInclude(x => x.IdRolNavigation).FirstOrDefaultAsync(x=>x.IdUsuario == id_usuario);

                if(usuario == null)
                {
                    resultado.Error = "El usuario no fue encontrado";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }

                resultado.Mensaje = "Usuario encontrado exitosamente";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;

                resultado.Resultado = new
                {
                    IdUsuario = usuario.IdUsuario,
                    Activo = usuario.Activo,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Roles = usuario.TRolesUsuarios.Select(s => s.IdRolNavigation.Descripcion).ToArray()
                };
            }
            catch (Exception)
            {
                resultado.Ok = false;
                resultado.CodigoEstado = 400;
                resultado.Error = "Error al obtener usuario";
            }

            return resultado;
        }
        private bool validarEmail(string email)
        {
            return email != null && Regex.IsMatch(email, "^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))@(([a-zA-Z]+[\\w-]+\\.){1,2}[a-zA-Z]{2,4})$");
        }
        private byte[] GetHash(string key)
        {
            var bytes = Encoding.UTF8.GetBytes(key);
            return new SHA256Managed().ComputeHash(bytes);
        }

        private string DecodeHash(byte[] key)
        {
            return Convert.ToBase64String(key);
        }
    }
}