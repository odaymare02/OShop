using OShop.API.Models;
using System.Linq.Expressions;

namespace OShop.API.Services.Products
{
    public interface IProductsServices
    {
        IEnumerable<Product> GetAll();
        Product Get(Expression<Func<Product, bool>> expression);
        Product Add(Product product);
        bool Edit(int id, Product product);
        bool Remove(int id);
    }
}
