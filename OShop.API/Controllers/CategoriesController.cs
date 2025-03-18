using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OShop.API.Data;
using OShop.API.Models;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        [HttpGet("")]
        public IActionResult GetAll()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            var Category = _context.Categories.Find(id);//do chashing when use find that mean when i request first time it get me after that make chase to this when request same thing it return from chash
            return Category == null ? NotFound() : Ok(Category);
        }

        [HttpPost("")]
        public IActionResult Create([FromBody]Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            //return Created($"{Request.Scheme}://{Request.Host}/");//201 mean success to add
            return CreatedAtAction("GetById", new { Id = category.Id }, category);
        }
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            _context.SaveChanges();
            return NoContent();//204
        }
    }
}
