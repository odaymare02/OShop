using OShop.API.Models;
using OShop.API.Services.IService;
using System.Linq.Expressions;

namespace OShop.API.Services.Categories
{
    public interface ICategoryServices:IService<Category>
    {
       // IEnumerable<Category>GetAll();//to retuen data in any kind i needed
        //Category Get(Expression<Func<Category,bool>> expression);//to handle get from any expresin like name staus id....
        //Task<Category> AddAsync(Category category,CancellationToken cancellationToken);
        Task<bool> EditAsync(int id, Category category,CancellationToken cancellationToken=default);
        Task<bool> UpdateStatusAsync(int id, CancellationToken cancellationToken=default);
        //Task<bool> RemoveAsync(int id,CancellationToken cancellationToken =default);

    }
}
