using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OShop.API.DTOs.Requests;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.Brands;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController(IBrandService brandService) : ControllerBase
    {
        private readonly IBrandService _brandService = brandService;
        [HttpGet("")]
        public IActionResult GetAll()
        {
            var Brands = _brandService.getAll();
            return Ok(Brands.Adapt<IEnumerable<BrandResponse>>());
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var brand = _brandService.Get(b => b.Id == id);
            return brand==null ? NotFound() : Ok(brand.Adapt<BrandResponse>());
        }
        [HttpPost("")]
        public IActionResult CreateBrand([FromBody]BrandRequest brand)
        {
            var brandinDb = _brandService.Add(brand.Adapt<Brand>());
            return CreatedAtAction("GetById",new { brandinDb.Id},brandinDb);
        }
        [HttpPut("{id}")]
        public IActionResult update([FromRoute]int id,[FromBody]BrandRequest brandReq)
        {
            bool progress = _brandService.Edit(id, brandReq.Adapt<Brand>());
            if (progress == false) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult delete([FromRoute]int id)
        {
            return _brandService.Remove(id) == false ? NotFound() : NoContent();
        }
        
    }
}
