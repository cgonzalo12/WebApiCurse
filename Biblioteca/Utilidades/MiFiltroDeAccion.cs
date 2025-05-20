using Microsoft.AspNetCore.Mvc.Filters;

namespace Biblioteca.Utilidades
{
    public class MiFiltroDeAccion : IActionFilter
    {
        private readonly ILogger<MiFiltroDeAccion> logger;

        public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> logger)
        {
            this.logger = logger;
        }
        //antes de la acion
        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Ejecutando el filtro de acción antes de la acción");
        }

        //despues de la acion
        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("Ejecutando el filtro de acción después de la acción");
        }

        
    }
}
