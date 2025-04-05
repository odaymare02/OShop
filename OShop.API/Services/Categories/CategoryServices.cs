using Microsoft.EntityFrameworkCore;
using OShop.API.Data;
using OShop.API.Models;
using System.Linq.Expressions;

namespace OShop.API.Services.Categories
{
    public class CategoryServices : ICategoryServices
    {
        ApplicationDbContext _context;
        public CategoryServices(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        public Category Add(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public bool Edit(int id, Category category)
        {
            Category? CategoryInDb = _context.Categories.AsNoTracking().FirstOrDefault(c => c.Id == id);
            if (CategoryInDb == null) return false;
            category.Id = id;
            _context.Categories.Update(category);
            _context.SaveChanges();
            return true;
        }

        public Category?/*to allow return nullable value*/ Get(Expression<Func<Category, bool>> expression)
        {
            return _context.Categories.FirstOrDefault(expression);
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        public bool Remove(int id)
        {
            Category? categoryInDb = _context.Categories.Find(id);
            if (categoryInDb == null) return false;
            _context.Remove(categoryInDb);
            _context.SaveChanges();
            return true;
        }
    }
}
