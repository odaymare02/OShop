using OShop.API.Models;
using System.Linq.Expressions;

namespace OShop.API.Services.Brands
{
    public interface IBrandService
    {
        IEnumerable<Brand> getAll();
        Brand Get(Expression<Func<Brand, bool>> expression);
        Brand Add(Brand brand);
        bool Edit(int id, Brand brand);
        bool Remove(int id);
    }
}
