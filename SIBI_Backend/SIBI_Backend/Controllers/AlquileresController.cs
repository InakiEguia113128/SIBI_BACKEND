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
    }
}
