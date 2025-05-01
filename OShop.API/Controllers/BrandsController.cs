using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OShop.API.DTOs.Requests;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.Brands;
using OShop.API.Utality;
using System.Threading.Tasks;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =$"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]
    public class BrandsController(IBrandService brandService) : ControllerBase
    {
        private readonly IBrandService _brandService = brandService;
        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var Brands = await _brandService.GetAsync();
            return Ok(Brands.Adapt<IEnumerable<BrandResponse>>());
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var brand = await _brandService.GetOne(b => b.Id == id);
            return brand==null ? NotFound() : Ok(brand.Adapt<BrandResponse>());
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateBrand([FromBody]BrandRequest brand)
        {
            var brandinDb =await _brandService.AddAsync(brand.Adapt<Brand>());
            return CreatedAtAction("GetById",new { brandinDb.Id},brandinDb.Adapt<BrandResponse>());
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> update([FromRoute]int id,[FromBody]BrandRequest brandReq,CancellationToken cancellationToken)
        {
            bool progress = await _brandService.EditAsync(id, brandReq.Adapt<Brand>(),cancellationToken);
            if (progress == false) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> delete([FromRoute]int id,CancellationToken cancellationToken)
        {
            return await _brandService.RemoveAsync(id,cancellationToken) == false ? NotFound() : NoContent();
        }
        
    }
}
