using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Biblioteca.Utilidades
{
    public static class ModelStateDiccionaryExtensions
    {
        public static BadRequestObjectResult ConstruirProblemDetail(this ModelStateDictionary modelState)
        {
            var problemDetail = new ValidationProblemDetails(modelState)
            {
                Title= "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest,
            };
            return new BadRequestObjectResult(problemDetail);
        }

    }
}
