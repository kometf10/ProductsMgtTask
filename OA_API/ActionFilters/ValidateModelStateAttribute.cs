using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using OA.Domin.Resources;
using OA.Domin.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OA_API.ActionFilters
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == HttpMethod.Post.Method || context.HttpContext.Request.Method == HttpMethod.Put.Method)
            {
                if (!context.ModelState.IsValid)
                {
                    //a way to do data annotation localization on server side
                    //var Localizer = (IStringLocalizer<CommonResources>)context.HttpContext.RequestServices.GetService(typeof(IStringLocalizer<CommonResources>));

                    //context.HttpContext.Response.Headers.Add("ResponseType", "ValidationErrors");

                    var response = context.ModelState
                                        .Where(modelError => modelError.Value.Errors.Count > 0)
                                        .Select(modelError => new ValidationResult
                                        {
                                            Field = modelError.Key,
                                            Errors = modelError.Value.Errors.Select(error => error.ErrorMessage).ToList()
                                        }).ToList();

                    context.Result = new BadRequestObjectResult(response);
                }

            }

        }

    }
}
