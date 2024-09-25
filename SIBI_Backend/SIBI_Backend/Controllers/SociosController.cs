using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIBI_Backend.Modelos.Socios;
using SIBI_Backend.Servicios.Socios;

namespace SIBI_Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SociosController : Controller
    {

        private readonly IServicioSocio servicioSocio;

        public SociosController(IServicioSocio _servicioSocio)
        {
            this.servicioSocio = _servicioSocio;
        }

        [HttpPost("registrar-socio/{idUsuario}")]
        public async Task<IActionResult> RegistrarSocio(Guid idUsuario,[FromBody] EntradaRegistrarSocio entrada)
        {
            var respuesta = await servicioSocio.RegistrarSocio(idUsuario,entrada);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPut("modificar-socio/{idUsuario}")]
        public async Task<IActionResult> ModificarSocio(Guid idUsuario, [FromBody] EntradaRegistrarSocio entrada)
        {
            var respuesta = await servicioSocio.ModificarSocio(idUsuario, entrada);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpGet("obtener-socio/{idUsuario}")]
        public async Task<IActionResult> ModificarSocio(Guid idUsuario)
        {
            var respuesta = await servicioSocio.ObtenerSocio(idUsuario);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPut("desactivar-socio/{idUsuario}")]
        public async Task<IActionResult> DesactivarSocio(Guid idUsuario)
        {
            var respuesta = await servicioSocio.DesactivarSocio(idUsuario);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPut("reactivar-socio/{idUsuario}")]
        public async Task<IActionResult> ReactivarSocio(Guid idUsuario)
        {
            var respuesta = await servicioSocio.ReactivarSocio(idUsuario);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}