using OShop.API.Models;
using System.Linq.Expressions;

namespace OShop.API.Services.Categories
{
    public interface ICategoryServices
    {
        IEnumerable<Category>GetAll();//to retuen data in any kind i needed
        Category Get(Expression<Func<Category,bool>> expression);//to handle get from any expresin like name staus id....
        Category Add(Category category);
        bool Edit(int id, Category category);
        bool Remove(int id);

    }
}
