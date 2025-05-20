using Biblioteca.Datos;
using Biblioteca.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Utilidades
{
    public class FiltroValidacionLibro : IAsyncActionFilter
    {
        private readonly ApplicationDbContext dbContext;

        public FiltroValidacionLibro(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //antes
            if (!context.ActionArguments.TryGetValue("libroCreacionDTO",out var value) ||
                value is not LibroCreacionDTO libroCreacionDTO)
            {
                context.ModelState.AddModelError(string.Empty, "el modelo enviado no es valido");
                context.Result = context.ModelState.ConstruirProblemDetail();
                return;
            }
            
            if (libroCreacionDTO.AutoresIds is null || libroCreacionDTO.AutoresIds.Count == 0)
            {
                context.ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), "No se puede crear un libro sin autores");
                context.Result = context.ModelState.ConstruirProblemDetail();
                return;
            }
            var autoresIdsExisten = await dbContext.Autores
                .Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id))
                .Select(x => x.Id).ToListAsync();
            if (autoresIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
            {
                var autoresNoExisten = libroCreacionDTO.AutoresIds.Except(autoresIdsExisten);
                var autoresnoexistenString = string.Join(",", autoresNoExisten);
                var mensajeDeError = $"Los siguientes autores no existen:{autoresnoexistenString}";
                context.ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), mensajeDeError);
                context.Result = context.ModelState.ConstruirProblemDetail();
                return;
            }
             

            await next();
        }
    }
}
