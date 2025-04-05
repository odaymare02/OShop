using Mapster;
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
                    img.CopyTo(fileStream);
                }
                product.MainImage = imgName;    
                _context.products.Add(product);
                _context.SaveChanges();
                return product;
            }

            return null;
        }

        public bool Edit(int id, Product product)
        {
            Product prod = _context.products.Find(id);
            prod.Adapt<ProductRequest>();
            var imgEdit = product.Adapt<ProductRequest>().MainImage;
            if (imgEdit != null && imgEdit.Length > 0)
            {
                var oldImgPath = Path.Combine(Directory.GetCurrentDirectory(), "images", prod.MainImage);
                if (System.IO.File.Exists(oldImgPath))
                {
                    File.Delete(oldImgPath);
                }
                var newImgName = Guid.NewGuid().ToString() + Path.GetExtension(imgEdit.FileName);
                var newImgPath = Path.Combine(Directory.GetCurrentDirectory() , "images", newImgName);
                using(var stream = System.IO.File.Create(newImgPath))
                {
                    imgEdit.CopyTo(stream);
                }
                prod.MainImage = newImgName;
            }
            prod.Name = product.Name;
            return true;
        }
        public Product Get(Expression<Func<Product, bool>> expression)
        {
            return _context.products.FirstOrDefault(expression);
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.products.ToList();
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
