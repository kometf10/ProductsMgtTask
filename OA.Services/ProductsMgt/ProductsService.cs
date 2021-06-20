using Microsoft.EntityFrameworkCore;
using OA.DataAccess;
using OA.Domin.ProductsMgt.Products;
using OA.Domin.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OA.Services.ProductsMgt
{
    public class ProductsService : IProductsService
    {
        private readonly AppDbContext DbContext;
        public ProductsService(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var products = await DbContext.Products.Include(p => p.ProductCatigories).ToListAsync();

            return products;

        }

        public async Task<Response<Product>> Get(int id)
        {
            var result = new Response<Product>();
            var product = await DbContext.Products.FindAsync(id);

            if(product == null)
            {
                result.HasErrors = true;
                result.AddValidationError("Id", "Product Not Found");
                return result;
            }

            result.Result = product;
            return result;
        }

        public async Task<Response<IEnumerable<Product>>> GetCatigoryProducts(int catigoryId)
        {
            var result = new Response<IEnumerable<Product>>();
            var catigory = await DbContext.Catigories.FindAsync(catigoryId);

            if (catigory == null)
            {
                result.HasErrors = true;
                result.AddValidationError("Id", "Catigory Not Found");
                return result;
            }

            var products = DbContext.ProductCatigories.Where(pc => pc.CatigoryId == catigoryId).Select(pc => pc.Product);
            result.Result = products;
            return result;
        }

        public async Task<Response<Product>> Create(Product product)
        {
            DbContext.Products.Add(product);
            _ = await DbContext.SaveChangesAsync();

            var result = new Response<Product>();
            result.Result = product;

            return result;
        }


        public async Task<Response<Product>> Update(Product product)
        {
            var existedProduct = await DbContext.Products.FindAsync(product.Id);

            var result = new Response<Product>();

            if (existedProduct == null)
            {
                result.HasErrors = true;
                result.AddValidationError("Id", "Product Not Found");
                return result;
            }

            existedProduct.Name = product.Name;
            existedProduct.Price = product.Price;
            existedProduct.ProductCatigories = product.ProductCatigories;

            _ = await DbContext.SaveChangesAsync();

            result.Result = existedProduct;
            return result;

        }

        public async Task<Response<string>> Delete(int id)
        {
            var product = await DbContext.Products.FindAsync(id);

            var result = new Response<string>();

            if (product == null)
            {
                result.HasErrors = true;
                result.AddValidationError("Id", "Product Not Found");
                return result;
            }

            DbContext.Products.Remove(product);
            _ = await DbContext.SaveChangesAsync();

            result.Result = "Deleted";
            return result;

        }


    }
}
