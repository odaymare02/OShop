using Microsoft.EntityFrameworkCore;
using OShop.API.Data;
using OShop.API.Models;
using System.Linq.Expressions;

namespace OShop.API.Services.Brands
{
    public class BrandsService : IBrandService
    {
        ApplicationDbContext _context;
        public BrandsService(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        public Brand Add(Brand brand)
        {
            _context.brands.Add(brand);
            _context.SaveChanges();
            return brand;
        }

        public bool Edit(int id, Brand brand)
        {
            Brand? brandInDb = _context.brands.AsNoTracking().FirstOrDefault(b => b.Id == id);
            if (brandInDb == null)
            {
                return false;
            }
            brand.Id = id;
            _context.brands.Update(brand);
            _context.SaveChanges();
            return true;
        }

        public Brand Get(Expression<Func<Brand, bool>> expression)
        {
            Brand ?brand = _context.brands.FirstOrDefault(expression);
            return brand;
        }

        public IEnumerable<Brand> getAll()
        {
            return _context.brands.ToList();
        }

        public bool Remove(int id)
        {
            Brand? BrandInDb = _context.brands.Find(id);
            if (BrandInDb == null)
            {
                return false;
            }
            _context.brands.Remove(BrandInDb);
            _context.SaveChanges();
            return true;
        }

       
    }
}
