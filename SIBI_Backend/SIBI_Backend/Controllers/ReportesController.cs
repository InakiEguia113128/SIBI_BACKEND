using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIBI_Backend.Modelos.Reportes;
using SIBI_Backend.Servicios.Reportes;

namespace SIBI_Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReportesController : Controller
    {
        private readonly IServicioReportes servicioReportes;

        public ReportesController(IServicioReportes _servicioReportes)
        {
            this.servicioReportes = _servicioReportes;
        }

        [HttpPut("libros-genero")]
        public async Task<IActionResult> RegistrarSocio([FromBody] EntradaReporteLibrosAlquiladosPorGenero entrada)
        {
            var respuesta = await servicioReportes.LibrosAlquiladosPorGenero(entrada);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}