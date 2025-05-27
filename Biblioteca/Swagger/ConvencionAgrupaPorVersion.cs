using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Biblioteca.Swagger
{
    public class ConvencionAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            //ejemplo: "controller.v1"
            var namespaceDelControlador = controller.ControllerType.Namespace;
            var version = namespaceDelControlador?.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = version;
        }
    }
}
