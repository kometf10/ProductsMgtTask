using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OA.DataAccess;
using OA.Domin.ProductsMgt.Catigories;
using OA.Domin.ProductsMgt.ProductCatigories;
using OA.Domin.ProductsMgt.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /*
     this controller for testing purposes only
     **/
    public class SeedDataController : ControllerBase
    {
        private readonly AppDbContext DbContext;
        private readonly UserManager<User> UserManager;
        public SeedDataController(AppDbContext dbContext, UserManager<User> userManager)
        {
            DbContext = dbContext;
            UserManager = userManager;
        }

        //Seed Admin User With Admin Role    
        [HttpGet("SeedUser")]
        public async Task<IActionResult> SeedUser()
        {
            var adminEmail = "Admin@app.com";
            var existed = DbContext.Users.FirstOrDefault(c => c.Email == adminEmail);

            if (existed != null)
                return Ok();

            var adminUser = new User
            {
                Email = adminEmail,
                UserName = "Admin"
            };

            var adminPassword = "Admin@123";

            var result = await UserManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                DbContext.Roles.Add(new IdentityRole() {Name = "Admin", NormalizedName = "ADMIN" } );
                DbContext.SaveChanges();

                await UserManager.AddToRoleAsync(adminUser, "Admin");
            }

            return Ok();

        }    

        //Seed Products Managments
        [HttpGet("SeedProducts")]
        public async Task<IActionResult> SeedProducts()
        {
            var existed = DbContext.Products.FirstOrDefault();

            if (existed != null)
                return Ok();

            var catigories = new List<Catigory>
            {
                new Catigory { Name = "Catigory 1"},
                new Catigory { Name = "Catigory 2"},
                new Catigory { Name = "Catigory 3"},
            };

            DbContext.Catigories.AddRange(catigories);

            var products = new List<Product>
            {
                new Product { Name = "Product 1", Price = 1, ProductCatigories = new List<ProductCatigory> { new ProductCatigory { Catigory = catigories.FirstOrDefault(c => c.Name == "Catigory 1") } } },
                new Product { Name = "Product 2", Price = 2, ProductCatigories = new List<ProductCatigory> { new ProductCatigory { Catigory = catigories.FirstOrDefault(c => c.Name == "Catigory 2") } } },
                new Product { Name = "Product 3", Price = 3, ProductCatigories = new List<ProductCatigory> { new ProductCatigory { Catigory = catigories.FirstOrDefault(c => c.Name == "Catigory 3") }, 
                                                                                                             new ProductCatigory { Catigory = catigories.FirstOrDefault(c => c.Name == "Catigory 1") } } },
            };

            DbContext.Products.AddRange(products);
                       
            await DbContext.SaveChangesAsync();
            
            return Ok();
        }

    }
}
