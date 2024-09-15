using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Comunes;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos.Usuarios;
using System.Text;
using System.Text.RegularExpressions;
using Web.Api.Softijs.Results;
using System.Security.Cryptography;

namespace SIBI_Backend.Servicios.Usuarios
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly SibiDbContext context;
        public ServicioUsuario(SibiDbContext _context)
        {
            this.context = _context;
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
