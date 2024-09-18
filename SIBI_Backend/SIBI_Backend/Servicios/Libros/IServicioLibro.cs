﻿using SIBI_Backend.Modelos.Libros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Api.Softijs.Results;

namespace SIBI_Backend.Servicios.Libros
{
    public interface IServicioLibro
    {
        Task<ResultadoBase> RegistrarLibro(EntradaRegistrarLibro entrada);
        Task<ResultadoBase> ObtenerGeneros();
    }
}
