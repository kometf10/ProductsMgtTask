using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domin.ProductsMgt.Catigories;
using OA.Services.ProductsMgt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OA_API.Controllers.ProductsMgt
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "catigories-managment")]
    public class CatigoriesController : ControllerBase
    {
        private readonly ICatigoriesService CatigoriesService;
        public CatigoriesController(ICatigoriesService catigoriesService)
        {
            CatigoriesService = catigoriesService;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await CatigoriesService.GetAll();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await CatigoriesService.Get(id);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Catigory catigory)
        {
            var result = await CatigoriesService.Create(catigory);

            return Ok(result);

        }

        [HttpPut]
        public async Task<IActionResult> Update(Catigory catigory)
        {
            var result = await CatigoriesService.Update(catigory);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await CatigoriesService.Delete(id);

            return Ok(result);
        }

    }
}
