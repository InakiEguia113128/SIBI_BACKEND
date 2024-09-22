using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIBI_Backend.Modelos.Libros;
using SIBI_Backend.Modelos.Usuarios;
using SIBI_Backend.Servicios.Libros;
using SIBI_Backend.Servicios.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIBI_Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LibrosController : Controller
    {
        private readonly IServicioLibro servicioLibros;

        public LibrosController(IServicioLibro _servicioLibros) 
        {
            this.servicioLibros = _servicioLibros;
        }

        [HttpPost("registrar-libro")]
        public async Task<IActionResult> RegistrarLibro([FromBody] EntradaRegistrarLibro entrada)
        {
            var respuesta = await servicioLibros.RegistrarLibro(entrada);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpGet("generos-libro")]
        public async Task<IActionResult> ObtenerGeneros()
        {
            var respuesta = await servicioLibros.ObtenerGeneros();

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpPost("obtener-catalogo")]
        public async Task<IActionResult> ObtenerCatalogo([FromBody] EntradaObtenerCatalogo entrada)
        {
            var respuesta = await servicioLibros.ObtenerCatalogo(entrada);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        [HttpDelete("eliminar-libro/{idLibro}")]
        public async Task<IActionResult> ElminarLibro(Guid idLibro)
        {
            var respuesta = await servicioLibros.ElminarLibro(idLibro);

            if (!respuesta.Ok)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
