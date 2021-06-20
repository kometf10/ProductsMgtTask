using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OA.Domin.ProductsMgt.Products;
using OA.Services.ProductsMgt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OA_API.Controllers.ProductsMgt
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "products-managment")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService ProductsService;
        public ProductsController(IProductsService productsService)
        {
            ProductsService = productsService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await ProductsService.GetAll();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await ProductsService.Get(id);

            return Ok(result);
        }

        [HttpGet("GetCatigoryProducts/{catigoryId}")]
        public async Task<IActionResult> GetCatigoryProducts(int catigoryId)
        {
            var result = await ProductsService.GetCatigoryProducts(catigoryId);

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Product product)
        {
            var result = await ProductsService.Create(product);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]Product product)
        {
            var result = await ProductsService.Update(product);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await ProductsService.Delete(id);

            return Ok(result);
        }

    }
}
