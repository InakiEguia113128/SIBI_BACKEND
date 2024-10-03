using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.EntityFrameworkCore;
using SIBI_Backend.Data;
using SIBI_Backend.Modelos;
using SIBI_Backend.Modelos.Alquileres;
using System.Collections.Generic;

namespace SIBI_Backend.Servicios.MercadoPago
{
    public class ServicioMercadoPago : IServicioMercadoPago
    {
        private readonly SibiDbContext context;

        public ServicioMercadoPago(SibiDbContext _contex)
        {
            this.context = _contex;
        }

        public async Task<ResultadoBase> CrearPreferencia(EntradaAlquiler entrada)
        {
            var resultado = new ResultadoBase();
            var lista_preferencia = new List<PreferenceItemRequest>();

            MercadoPagoConfig.AccessToken = "TEST-2016235320474598-100209-108336d1282e8bda72e7614513802b2a-335183307";

            try
            {
                var usuario = await context.TUsuarios.FirstOrDefaultAsync(x => x.IdUsuario == entrada.idSocio);

                foreach (var detalleAlquiler in entrada.detalleAlquiler)
                {
                    var libro = await context.TLibros.FirstOrDefaultAsync(x=>x.IdLibro == detalleAlquiler.idLibro);

                    lista_preferencia.Add(new PreferenceItemRequest
                    {
                        Title = libro.Titulo,
                        Quantity = 1,
                        CurrencyId = "ARS",
                        UnitPrice = detalleAlquiler.subtotalDetalle 
                    });
                }

                var request = new PreferenceRequest
                {
                    Items = lista_preferencia,
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = "https://tusitio.com/success",
                        Failure = "https://tusitio.com/failure",
                        Pending = "https://tusitio.com/pending"
                    },
                    AutoReturn = "approved",
                    AdditionalInfo = $"Alquiler de libros para el socio {usuario.Nombre} {usuario.Apellido}"
                };

                var client = new PreferenceClient();
                Preference preference = client.Create(request);

                resultado.Ok = true;
                resultado.CodigoEstado = 200;
                resultado.Resultado = new { init_point = preference.InitPoint };
                resultado.Mensaje = "Preferencia de MP creada";

            }
            catch (Exception)
            {
                resultado.Ok = false;
                resultado.CodigoEstado = 500;
                resultado.Error = "Error al crear preferencia de MP";
            }

            return resultado;
        }
    }
}