﻿using Biblioteca.DTOs;
using Biblioteca.Servicios.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Biblioteca.Utilidades.V1
{
    public class HATEOASAutorAttribute:HATEOASFilterAttribute
    {
        private readonly IGeneradorEnlaces generadorEnlaces;

        public HATEOASAutorAttribute(IGeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context,ResultExecutionDelegate next)
        {
            var incluirHATEOAS = DebeIncluirHATEOAS(context);
            if (!incluirHATEOAS)
            {
                await next();
                return;
            }
            var result = context.Result as ObjectResult;
            var modelo = result!.Value as AutorDTO ?? throw new ArgumentNullException("Se esperaba una instacia de AutorDTO");
            await generadorEnlaces.GenerarEnlaces(modelo);
            await next();
        }
    }
}
