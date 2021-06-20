using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using OA.DataAccess;
using OA.Domin.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OA_API.Controllers
{
    [AllowAnonymous]
    [Route("Error")]
    public class ErrorController : ControllerBase
    {

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IStringLocalizer<ErrorController> localizer;
        private readonly AppDbContext dbContext;
        public ErrorController(IHttpContextAccessor httpContextAccessor, IStringLocalizer<ErrorController> localizer, AppDbContext dbContext)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.localizer = localizer;
            this.dbContext = dbContext;
        }

        [Route("")]
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Get()
        {
            var exceptionDetails = httpContextAccessor.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ExceptionLog e = new ExceptionLog()
            {
                Path = exceptionDetails.Path,
                Message = exceptionDetails.Error.Message,
                StackTrace = exceptionDetails.Error.StackTrace
            };
            dbContext.ExceptionLogs.Add(e);
            dbContext.SaveChanges();


            return StatusCode(StatusCodes.Status500InternalServerError, new { Msg = localizer["UNEXPECTED_ERROR"], Details = exceptionDetails });
        }


    }
}
