using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIBI_Backend.Modelos.Alquileres;
using SIBI_Backend.Modelos.Libros;
using SIBI_Backend.Servicios.Alquileres;

namespace SIBI_Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AlquileresController : Controller
    {
        private readonly IServicioAlquiler servicioAlquiler;

        public AlquileresController(IServicioAlquiler _servicioAlquiler)
        {
            this.servicioAlquiler = _servicioAlquiler;
        }

        [HttpPost("registrar-alquiler")]
        public async Task<IActionResult> RegistrarLibro([FromBody] EntradaAlquiler entrada)
        {
            var respuesta = await servicioAlquiler.RegistrarAlquiler(entrada);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPost("obtener-alquileres")]
        public async Task<IActionResult> ObtenerAlquileres([FromBody] EntradaObtenerAlquileres entrada)
        {
            var respuesta = await servicioAlquiler.ObtenerAquileres(entrada);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpGet("obtener-alquiler/{id}")]
        public async Task<IActionResult> ObtenerAlquiler(Guid id)
        {
            var respuesta = await servicioAlquiler.ObtenerAlquilerPorId(id);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpGet("obtener-estados-alquiler")]
        public async Task<IActionResult> ObtenerEstadosAlquiler()
        {
            var respuesta = await servicioAlquiler.ObtenerEstadosAlquiler();

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
