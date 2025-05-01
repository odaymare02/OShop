using Microsoft.EntityFrameworkCore;
using OShop.API.Data;
using OShop.API.Models;
using OShop.API.Services.IService;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OShop.API.Services.Categories
{
    public class CategoryServices : Service<Category>,ICategoryServices
    {
        ApplicationDbContext _context;
        public CategoryServices(ApplicationDbContext context):base(context)//by default the class that i inhirete from it this constructore call the default constructor of the parent but the default constructor deleted cuz i make another constructore i should call the nother constructore 
        {
            this._context = context;
        }
     

        public async Task<bool> EditAsync(int id, Category category,CancellationToken cancellationToken=default)
        {
            Category? CategoryInDb = _context.Categories.Find(id);
            if (CategoryInDb == null) return false;
            CategoryInDb.Name = category.Name;
            CategoryInDb.Description = category.Description;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> UpdateStatusAsync(int id,CancellationToken cancellationToken)
        {
            var catInDb = _context.Categories.Find(id);
            if (catInDb == null) return false;
            catInDb.Status = !catInDb.Status;
           await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
   
    }
}
