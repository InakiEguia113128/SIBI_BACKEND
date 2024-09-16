using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Comunes;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos.Usuarios;
using System.Text;
using System.Text.RegularExpressions;
using Web.Api.Softijs.Results;
using System.Security.Cryptography;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SIBI_Backend.Servicios.Usuarios
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly SibiDbContext context;
        private readonly IConfiguration config;
        public ServicioUsuario(SibiDbContext _context, IConfiguration _config)
        {
            this.context = _context;
            this.config = _config;
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
                    NombreCompleto = entrada.nombreCompleto,
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
                     new Claim(ClaimTypes.Name, usuario.NombreCompleto),
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
                    IdUsuario = usuario.IdUsuario,
                    Activo = usuario.Activo,
                    Email = usuario.Email,
                    Nombre = usuario.NombreCompleto,
                    Roles = usuario.TRolesUsuarios.Select(s => s.IdRolNavigation.Descripcion).ToArray(),
                    Token = token
                };

                respuesta.Mensaje = "Sesión iniciada exitosamente";
                respuesta.Ok = true;
                respuesta.CodigoEstado = 200;
                respuesta.Resultado = tokenHandler.WriteToken(token);
            }
            catch (Exception)
            {
                respuesta.Ok = false;
                respuesta.CodigoEstado = 400;
                respuesta.Error = "Error al iniciar sesión";
            }

            return respuesta;
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
    }
}
