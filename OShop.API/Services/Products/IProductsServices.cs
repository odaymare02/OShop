using OShop.API.DTOs.Requests;
using OShop.API.Models;
using System.Linq.Expressions;

namespace OShop.API.Services.Products
{
    public interface IProductsServices
    {
        IEnumerable<Product> GetAll(string qyery,int page,int limit);
        Product Get(Expression<Func<Product, bool>> expression);
        Product Add(ProductRequest product);
        bool Edit(int id, ProductUpdateRequest product);
        bool Remove(int id);
    }
}
