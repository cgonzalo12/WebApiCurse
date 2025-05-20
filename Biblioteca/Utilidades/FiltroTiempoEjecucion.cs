using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Biblioteca.Utilidades
{
    public class FiltroTiempoEjecucion : IAsyncActionFilter
    {
        private readonly ILogger<FiltroTiempoEjecucion> logger;

        public FiltroTiempoEjecucion(ILogger<FiltroTiempoEjecucion> logger)
        {
            this.logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //antes de la ejecucion de la accion
            var stopWatch= Stopwatch.StartNew();
            logger.LogInformation($"INICIO Accion:{context.ActionDescriptor.DisplayName}");
            await next();
            //despues de la ejecucion
            stopWatch.Stop();
            logger.LogInformation($"FIN Accion:{context.ActionDescriptor.DisplayName} - TIEMPO {stopWatch.ElapsedMilliseconds} ms");

        }
    }
}
