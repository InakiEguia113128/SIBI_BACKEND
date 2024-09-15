using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos.Usuarios;
using System.Text.RegularExpressions;
using Web.Api.Softijs.Results;

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
                    resultado.Error = "El email ingresado  posee un formato incorrecto";
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;

                    return resultado;
                }



                await context.TUsuarios.AddAsync(new TUsuario()
                {
                    IdUsuario = Guid.NewGuid(),
                    NombreCompleto = entrada.nombreCompleto,
                    Email = entrada.email,
                    FechaCreacion = DateOnly.FromDateTime(DateTime.Now)
                    //Activo = true
                });


            }
            catch (Exception)
            {

                throw;
            }

            return resultado;
        }

        private bool validarEmail(string email)
        {
            return email != null && Regex.IsMatch(email, "^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))@(([a-zA-Z]+[\\w-]+\\.){1,2}[a-zA-Z]{2,4})$");
        }
    }
}
