using Mapster;
using Microsoft.EntityFrameworkCore;
using OShop.API.Data;
using OShop.API.DTOs.Requests;
using OShop.API.DTOs.Responses;
using OShop.API.Models;
using System.Linq.Expressions;

namespace OShop.API.Services.Products
{

    public class ProductsServices(ApplicationDbContext context) : IProductsServices
    {
        private readonly ApplicationDbContext _context = context;
        public Product Add(Product product)
        {
            var img = product.Adapt<ProductRequest>().MainImage;
            if (img != null && img.Length > 0)
            {
                var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "images", imgName);

                using (var fileStream = new FileStream(imgPath, FileMode.Create))
                {
                    img.CopyToAsync(fileStream);
                }
                product.MainImage = imgName;    
                _context.products.Add(product);
                _context.SaveChanges();
                return product;
            }

            return null;
        }

        public bool Edit(int id, ProductUpdateRequest request)
        {
            Product prodInDb = _context.products.AsNoTracking().FirstOrDefault(p=>p.Id==id);
            var product = request.Adapt<Product>();
            var imgEdit = request.MainImage;
            
           
            if (prodInDb != null)
            {
                if (imgEdit != null && imgEdit.Length > 0)
                {
                    var oldImgPath = Path.Combine(Directory.GetCurrentDirectory(), "images", prodInDb.MainImage);
                    if (File.Exists(oldImgPath))
                    {
                        File.Delete(oldImgPath);
                    }
                    var newImgName = Guid.NewGuid().ToString() + Path.GetExtension(imgEdit.FileName);
                    var newImgPath = Path.Combine(Directory.GetCurrentDirectory(), "images", newImgName);
                    using (var stream = System.IO.File.Create(newImgPath))
                    {
                        imgEdit.CopyToAsync(stream);
                    }
                    product.MainImage = newImgName;
                }
                else//don't need to edit the image
                {
                    product.MainImage = prodInDb.MainImage;
                }
                product.Id = id;
                _context.products.Update(product);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public Product Get(Expression<Func<Product, bool>> expression)
        {
            return _context.products.FirstOrDefault(expression);
        }

        public IEnumerable<Product> GetAll(string query,int page,int limit)
        {
            IQueryable<Product> products = _context.products;//جبلي ياهن على مستوى السيرفر انا بفلتر بعدها حطهن عندي
            if (page <= 0 || limit < 0)
            {
                page = 1;
                limit = 10;
            }
            if (query != null)//serach about some product
            {
                products = products.Where(p => p.Name.Contains(query) || p.Description.Contains(query));
            }//return all product with same name now i will do pagination
            products = products.Skip((page - 1) * limit).Take(limit);
            return products;

        }

        public bool Remove(int id)
        {
            Product prod = _context.products.Find(id);
            if (prod == null) return false;
            var img = prod.MainImage;
            if (img != null && img.Length > 0)
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "images", img);
                System.IO.File.Delete(imgPath);
            }
            _context.products.Remove(prod);
            _context.SaveChanges();
            return true;
        }

       
    }
}
