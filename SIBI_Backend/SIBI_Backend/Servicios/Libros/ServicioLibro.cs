using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos.Libros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Softijs.Results;

namespace SIBI_Backend.Servicios.Libros
{
    public class ServicioLibro : IServicioLibro
    {
        private readonly SibiDbContext context;
        private readonly IConfiguration config;
        public ServicioLibro(SibiDbContext _context, IConfiguration _config)
        {
            this.context = _context;
            this.config = _config;
        }
        public async Task<ResultadoBase> RegistrarLibro(EntradaRegistrarLibro entrada)
        {
            var resultado = new ResultadoBase();

            try
            {
                if(await context.TLibros.AnyAsync(x => x.CodigoIsbn == entrada.codigoIsbn))
                {
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;
                    resultado.Error = "El código ISBN ya se encuentra registrado, ingrese un código válido";

                    return resultado;
                }

                if (!await context.TGenerosLibros.AnyAsync(x => x.IdGeneroLibro == entrada.idGenero)) 
                {
                    resultado.Ok = false;
                    resultado.CodigoEstado = 400;
                    resultado.Error = "El género con el que se intenta registar el libro no existe, ingrese un género válido";

                    return resultado;
                }

                await context.TLibros.AddAsync(new TLibro()
                {
                    IdLibro = Guid.NewGuid(),
                    CodigoIsbn = entrada.codigoIsbn,
                    Titulo = entrada.titulo,
                    CantidadEjemplares = entrada.cantidadEjemplares,
                    Descripcion = entrada.descripcion,
                    NombreAutor = entrada.nombreAutor,
                    Editorial = entrada.editorial,
                    NroEdicion = entrada.nroEdicion,
                    NroVolumen = entrada.nroVolumen,
                    CodUbicacion = entrada.codUbicacion,
                    IdGenero = entrada.idGenero,
                    NGenero = entrada.nGenero,
                    FechaPublicacion = entrada.fechaPublicacion,
                    ImagenPortadaBase64 = entrada.imagenPortadaBase64,
                    Activo = true,
                    FechaCreacion = DateOnly.FromDateTime(DateTime.Now),
                    PrecioAlquiler = entrada.precioAlquiler
                });

                await context.SaveChangesAsync();

                resultado.Mensaje = "Libro registrado con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
            }
            catch (Exception)
            {
                resultado.Error = "Error al registrar libro";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> ObtenerGeneros()
        {
            var resultado = new ResultadoBase();

            try
            {
                var generos = await context.TGenerosLibros.Select(x => new { idGenero = x.IdGeneroLibro, descripcion = x.Descripcion }).ToListAsync();

                resultado.Mensaje = "Géneros recuperados con éxito";
                resultado.Ok = true;
                resultado.CodigoEstado = 200;
                resultado.Resultado = generos;
            }
            catch (Exception)
            {
                resultado.Error = "Error al obtener los géneros de libros";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public static bool ValidarISBN(string isbn)
        {
            isbn = isbn.Replace("-", "").Replace(" ", "");

            if (isbn.Length == 10)
            {
                return ValidarISBN10(isbn);
            }
            else if (isbn.Length == 13)
            {
                return ValidarISBN13(isbn);
            }
            return false;
        }


        private static bool ValidarISBN10(string isbn10)
        {
            if (isbn10.Length != 10) return false;

            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(isbn10[i])) return false;
                suma += (isbn10[i] - '0') * (10 - i);
            }


            char ultimoCaracter = isbn10[9];
            int digitoControl;
            if (ultimoCaracter == 'X') digitoControl = 10;
            else if (char.IsDigit(ultimoCaracter)) digitoControl = ultimoCaracter - '0';
            else return false;

            suma += digitoControl;

            return (suma % 11 == 0);
        }


        private static bool ValidarISBN13(string isbn13)
        {

            if (isbn13.Length != 13 || !isbn13.All(char.IsDigit)) return false;

            int suma = 0;
            for (int i = 0; i < 13; i++)
            {
                int digito = isbn13[i] - '0';

                suma += digito * (i % 2 == 0 ? 1 : 3);
            }

            return (suma % 10 == 0);
        }

        public async Task<ResultadoBase> ObtenerCatalogo(EntradaObtenerCatalogo entrada)
        {
            var resultado = new ResultadoBase();

            try
            {
                var consulta = context.TLibros.AsQueryable();

                if (!string.IsNullOrEmpty(entrada.titulo))
                {
                    var tituloLower = entrada.titulo.ToLower();
                    consulta = consulta.Where(x => x.Titulo.ToLower().Contains(tituloLower));
                }

                if (!string.IsNullOrEmpty(entrada.autos))
                {
                    var autorLower = entrada.autos.ToLower();
                    consulta = consulta.Where(x => x.NombreAutor.ToLower().Contains(autorLower));
                }

                if (!string.IsNullOrEmpty(entrada.editorial))
                {
                    var editorialLower = entrada.editorial.ToLower();
                    consulta = consulta.Where(x => x.Editorial.ToLower().Contains(editorialLower));
                }

                if (!string.IsNullOrEmpty(entrada.ISBN))
                {
                    var isbnLower = entrada.ISBN.ToLower();
                    consulta = consulta.Where(x => x.CodigoIsbn.ToLower().Contains(isbnLower));
                }

                if (!string.IsNullOrEmpty(entrada.nGenero))
                {
                    var nGeneroLower = entrada.nGenero.ToLower();
                    consulta = consulta.Where(x => x.NGenero.ToLower().Contains(nGeneroLower));
                }

                if (entrada.idGenero.HasValue)
                {
                    consulta = consulta.Where(x => x.IdGenero == entrada.idGenero.Value);
                }

                if(entrada.fechaPublicacionDesde.HasValue && entrada.fechaPublicacionHasta.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaPublicacion >= DateOnly.FromDateTime(entrada.fechaPublicacionDesde.Value) && x.FechaPublicacion <= DateOnly.FromDateTime(entrada.fechaPublicacionHasta.Value));
                }
                else if (entrada.fechaPublicacionDesde.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaPublicacion >= DateOnly.FromDateTime(entrada.fechaPublicacionDesde.Value));
                }
                else if (entrada.fechaPublicacionHasta.HasValue)
                {
                    consulta = consulta.Where(x => x.FechaPublicacion <= DateOnly.FromDateTime(entrada.fechaPublicacionHasta.Value));
                }

                if (entrada.precioDesde.HasValue)
                {
                    consulta = consulta.Where(x => x.PrecioAlquiler >= entrada.precioDesde.Value);
                }

                if (entrada.precioHasta.HasValue)
                {
                    consulta = consulta.Where(x => x.PrecioAlquiler <= entrada.precioHasta.Value);
                }

                //Solo se devuelven los activos
                consulta = consulta.Where(libro => libro.Activo == true);
                consulta = consulta.OrderByDescending(l => l.FechaCreacion);

                // Paginación

                consulta = consulta.Skip(entrada.salta);
                consulta = consulta.Take(entrada.devolver);

                var libros = await consulta.ToListAsync();

                resultado.Ok = true;
                resultado.Mensaje = "Catálogo recuperado con éxito";
                resultado.Resultado = libros;
                resultado.CodigoEstado = 200;
            }
            catch (Exception)
            {
                resultado.Error = "Error al obtener catálogo libros";
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
            }

            return resultado;
        }

        public async Task<ResultadoBase> ElminarLibro(Guid idLibro)
        {
            var salida = new ResultadoBase();

            try
            {
                var libro = await context.TLibros.FirstOrDefaultAsync(x=>x.IdLibro == idLibro);

                if (libro == null) 
                {
                    salida.Error = "El libro no existe";
                    salida.CodigoEstado = 400;
                    salida.Ok = false;
                }

                libro.Activo = false;
                
                await context.SaveChangesAsync();               
            }
            catch (Exception)
            {
                salida.Error = "Error el eliminar el libro";
                salida.CodigoEstado = 500;
                salida.Ok = false;
            }

            salida.Mensaje = "Libro eliminado correctamente";
            salida.CodigoEstado = 200;
            salida.Ok = true;

            return salida;
        }
    }
}