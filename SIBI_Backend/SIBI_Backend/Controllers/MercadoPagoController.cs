using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIBI_Backend.Modelos.Alquileres;
using SIBI_Backend.Modelos.Socios;
using SIBI_Backend.Servicios.MercadoPago;
using SIBI_Backend.Servicios.Socios;

namespace SIBI_Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MercadoPagoController : Controller
    {
        private readonly IServicioMercadoPago servicioMercadoPago;

        public MercadoPagoController(IServicioMercadoPago _servicioMercadoPago)
        {
            this.servicioMercadoPago = _servicioMercadoPago;
        }

        [HttpPost("crear-preferencia")]
        public async Task<IActionResult> RegistrarSocio([FromBody] EntradaAlquiler entrada)
        {
            var respuesta = await servicioMercadoPago.CrearPreferencia(entrada);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
