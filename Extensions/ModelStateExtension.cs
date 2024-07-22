using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Blog.Extensions
{
    public static class ModelStateExtension
    {
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            var resultErrosList = new List<string>();
            foreach(var item in modelState.Values)
                    resultErrosList.AddRange(item.Errors.Select(error => error.ErrorMessage));
            return resultErrosList;
        }
    }
}
