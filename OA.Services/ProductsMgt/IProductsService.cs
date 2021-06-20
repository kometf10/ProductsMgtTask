using OA.Domin.ProductsMgt.Products;
using OA.Domin.Responces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OA.Services.ProductsMgt
{
    public interface IProductsService
    {

        Task<IEnumerable<Product>> GetAll();

        Task<Response<Product>> Get(int id);

        Task<Response<IEnumerable<Product>>> GetCatigoryProducts(int catigoryId);

        Task<Response<Product>> Create(Product product);

        Task<Response<Product>> Update(Product product);

        Task<Response<string>> Delete(int id); 

    }
}
