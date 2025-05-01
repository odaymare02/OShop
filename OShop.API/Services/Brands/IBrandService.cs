using OShop.API.Models;
using OShop.API.Services.IService;
using System.Linq.Expressions;

namespace OShop.API.Services.Brands
{
    public interface IBrandService:IService<Brand>
    {
        //IEnumerable<Brand> getAll();
        //Brand Get(Expression<Func<Brand, bool>> expression);
        //Brand Add(Brand brand);
        Task<bool> EditAsync(int id, Brand brand,CancellationToken cancellationToken);
        Task<bool> UpdateStatus(int id, CancellationToken cancellationToken);
        //bool Remove(int id);
    }
}
