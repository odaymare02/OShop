using Microsoft.EntityFrameworkCore;
using OShop.API.Data;
using OShop.API.Migrations;
using OShop.API.Models;
using OShop.API.Services.IService;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OShop.API.Services.Brands
{
    public class BrandsService:Service<Brand>,IBrandService 
    {
        ApplicationDbContext _context;
        public BrandsService(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public async Task<bool> EditAsync(int id, Brand brand, CancellationToken cancellationToken=default)
        {
            Brand? BrandInDb =  await _context.brands.FindAsync(id);
            if (BrandInDb == null) return false;
            BrandInDb.Id = id;
            BrandInDb.Name = brand.Name;
            BrandInDb.Description = brand.Description;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateStatus(int id, CancellationToken cancellationToken)
        {
            Brand? brandInDb =await _context.brands.FindAsync(id);
            if (brandInDb == null)
            {
                return false;
            }
            brandInDb.Status = !brandInDb.Status;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        /*
public async Task<bool> EditAsync(int id, Brand brand,CancellationToken cancellationToken)
{
   Brand? brandInDb = _context.brands.Find(id);
   if (brandInDb == null)
   {
       return false;
   }
   brand.Id = id;
   brandInDb.Name = brand.Name;
   brandInDb.Descrition = brand.Descrition;
  await _context.SaveChangesAsync(cancellationToken);
   return true;
}

public async Task<bool> UpdateStatus(int id, CancellationToken cancellationToken)
{
   Brand? brandInDb = _context.brands.Find(id);
   if (brandInDb == null)
   {
       return false;
   }
   brandInDb.Status = !brandInDb.Status;
   await _context.SaveChangesAsync(cancellationToken);
   return true;
}
*/
    }
}
