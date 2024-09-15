using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIBI_Backend.Modelos.Usuarios;
using SIBI_Backend.Servicios.Usuarios;

namespace SIBI_Backend.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IServicioUsuario servicioUsuario;
        private readonly IConfiguration config;

        public UsuariosController(IServicioUsuario _servicioUsuario, IConfiguration _config)
        {
            this.servicioUsuario = _servicioUsuario;
            this.config = _config;
        }

        [HttpPost("registrar-usuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] EntradaRegistrarUsuario entrada)
        {
            var respuesta = await servicioUsuario.RegistrarUsuario(entrada);

            return Ok(respuesta);
        }
    }
}
