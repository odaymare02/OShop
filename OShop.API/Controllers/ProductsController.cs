using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OShop.API.DTOs.Requests;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.Products;
using OShop.API.Utality;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = $"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]//allow onlyuser with this role to access to all endpoint under the code expect the endpoint that write allowanonymous

    public class ProductsController(IProductsServices productsServices) : ControllerBase
    {
        private readonly IProductsServices _productsServices = productsServices;
        [HttpGet("")]
        [AllowAnonymous]//this mean allow anyuser to use this endpoint
        public IActionResult GetAll([FromQuery]string? query, [FromQuery] int page, [FromQuery]int limit=10)
        {
            var product= _productsServices.GetAll(query,page,limit);
            return Ok(product.Adapt<IEnumerable<ProductResponse>>());
        }
        [HttpGet("{id}")]
        [AllowAnonymous]//this mean allow anyuser to use this endpoint
        public IActionResult GetById([FromRoute] int id)
        {
            var product = _productsServices.Get(p=>p.Id==id);
            return product == null ? NotFound() : Ok(product.Adapt<ProductResponse>());
        }
        [HttpPost("")]
        public IActionResult Create([FromForm] ProductRequest productRequest)
        {
                var product = _productsServices.Add(productRequest);
                return CreatedAtAction("GetById", new { product.Id }, product.Adapt<ProductResponse>());
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
          
            bool result = _productsServices.Remove(id);
            return result == false ? NotFound() : NoContent();
        }
        [HttpPut("{id}")]
        public IActionResult update([FromRoute]int id, [FromForm]ProductUpdateRequest productRequest)
        {
            bool result = _productsServices.Edit(id, productRequest);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
      
    }
}
