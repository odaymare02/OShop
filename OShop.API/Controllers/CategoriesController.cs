﻿using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OShop.API.Data;
using OShop.API.DTOs.Requests;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using OShop.API.Services.Categories;
using OShop.API.Utality;
using System.Threading.Tasks;

namespace OShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]//this provid automatically line to check if the parameters that i put the data anotaion on it is valid or not (if(!ModelState.isvalid)return validationProblem((ModelSatate)))
    [Authorize]
    public class CategoriesController(ICategoryServices categoryServices) : ControllerBase
    {
       private readonly ICategoryServices _categoryServices=categoryServices;
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryServices.GetAsync();
            return Ok(categories.Adapt<IEnumerable <CategoryResponse>>());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var Category = await _categoryServices.GetOne(e => e.Id == id);//do chashing when use find that mean when i request first time it get me after that make chase to this when request same thing it return from chash
            return Category == null ? NotFound() : Ok(Category.Adapt<CategoryResponse>());
        }

        [HttpPost("")]
        [Authorize(Roles =$"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]
        public async Task<IActionResult> Create([FromBody]CategoryRequest categoryRequest,CancellationToken cancellationToken)
        {
            var catInDb= await _categoryServices.AddAsync(categoryRequest.Adapt<Category>(),cancellationToken);
            //return Created($"{Request.Scheme}://{Request.Host}/");//201 mean success to add
            return CreatedAtAction("GetById", new { Id = catInDb.Id }, catInDb);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = $"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]

        public async Task<IActionResult> update([FromRoute]int id,[FromBody]CategoryRequest categoryRequest,CancellationToken cancellationToken=default)
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
            var catInDb =await _categoryServices.EditAsync(id, categoryRequest.Adapt<Category>(),cancellationToken);
            if (!catInDb) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{StaticData.SuperAdmin},{StaticData.Admin},{StaticData.Company}")]

        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            //var category = _context.Categories.Find(id);
            //if (category == null) return NotFound();
            //_context.Categories.Remove(category);
            //_context.SaveChanges();
            //return NoContent();//204
            var catindb =await _categoryServices.RemoveAsync(id);
            if (!catindb) return NotFound();
            return NoContent();
        }
    }
}
