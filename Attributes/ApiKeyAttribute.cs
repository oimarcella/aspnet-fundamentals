using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Blog.Attributes
{
    //Para poder decorar uma classe        ou               um método
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(
        ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Query.TryGetValue(Configuration.ApiKeyName, out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "ApiKey não encontrada"
                };
                return;
            }

            if (!Configuration.ApiKeyValue.Equals(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Acesso não autorizado"
                };
                return;
            }

            // Execução continua automaticamente sem o next() chamado explicitamente

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Este método pode ser deixado vazio se não houver necessidade de executar lógica após a execução da ação
        }
    }
}
