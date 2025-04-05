using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OShop.API.Data;
using OShop.API.DTOs.Requests;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.Categories;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]//this provid automatically line to check if the parameters that i put the data anotaion on it is valid or not (if(!ModelState.isvalid)return validationProblem((ModelSatate)))
    public class CategoriesController(ICategoryServices categoryServices) : ControllerBase
    {
       private readonly ICategoryServices _categoryServices=categoryServices;
        [HttpGet("")]
        public IActionResult GetAll()
        {
            
            var categories = _categoryServices.GetAll();
            return Ok(categories.Adapt<IEnumerable <CategoryResponse>>());
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var Category = _categoryServices.Get(e=>e.Id==id).Adapt<CategoryResponse>();//do chashing when use find that mean when i request first time it get me after that make chase to this when request same thing it return from chash
            return Category == null ? NotFound() : Ok(Category);
        }

        [HttpPost("")]
        public IActionResult Create([FromBody]CategoryRequest categoryRequest)
        {
            var catInDb=_categoryServices.Add(categoryRequest.Adapt<Category>());
            //return Created($"{Request.Scheme}://{Request.Host}/");//201 mean success to add
            return CreatedAtAction("GetById", new { Id = catInDb.Id }, catInDb);
        }
        [HttpPut("{id}")]
        public IActionResult update([FromRoute]int id,[FromBody]CategoryRequest categoryRequest)
        {
            ////var catInDb = _context.Categories.Find(id);
            //var catInDb = _context.Categories.AsNoTracking().FirstOrDefault(c => c.Id == id);
            //if (catInDb == null) return NotFound();
            //category.Id = id;
            //// _context.Categories.Update(category);
            //// catInDb.Name = category.Name;
            ////catInDb.Description = category.Description;
            //_context.Categories.Update(category);
            //_context.SaveChanges();
            //return NoContent();
            var catInDb = _categoryServices.Edit(id, categoryRequest.Adapt<Category>());
            if (!catInDb) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            //var category = _context.Categories.Find(id);
            //if (category == null) return NotFound();
            //_context.Categories.Remove(category);
            //_context.SaveChanges();
            //return NoContent();//204
            var catindb = _categoryServices.Remove(id);
            if (!catindb) return NotFound();
            return NoContent();
        }
    }
}
