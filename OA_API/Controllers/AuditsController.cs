using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OA.DataAccess;
using OA.Domin.Auditing;
using OA.Domin.Paging;
using OA.Domin.RequestFeatures;
using OA.Domin.Requests;

namespace OA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "audeting-access")]
    public class AuditsController : ControllerBase
    {

        private readonly AppDbContext dbContext;
        public AuditsController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("GetAudits")]
        public IActionResult GetAudits([FromBody] AuditSearchRequest auditSearchRequest, [FromQuery] RequestParameters pagingParameters)
        {
            if (auditSearchRequest.FromDate == null)
                auditSearchRequest.FromDate = DateTime.Now.AddDays(-1);
            if (auditSearchRequest.ToDate == null)
                auditSearchRequest.ToDate = DateTime.Now.AddDays(1);
            //Required Filters
            var audits = dbContext.Audits.Where(a => a.Date >= auditSearchRequest.FromDate && a.Date <= auditSearchRequest.ToDate && a.TableName == auditSearchRequest.TableName);

            //Optional Filters
            if (!string.IsNullOrEmpty(auditSearchRequest.UserId))
                audits = audits.Where(a => a.UserId == auditSearchRequest.UserId);
            if (!string.IsNullOrEmpty(auditSearchRequest.Operation))
                audits = audits.Where(a => a.Operation == auditSearchRequest.Operation);

            audits = audits.OrderByDescending(a => a.Id);

            var resultList = PagedList<Audit>.ToPagedList(audits, pagingParameters.PageNumber, pagingParameters.PageSize);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(resultList.PagingData));

            return Ok(resultList);

        }

    }
}
