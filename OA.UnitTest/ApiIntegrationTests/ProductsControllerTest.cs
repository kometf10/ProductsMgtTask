using FluentAssertions;
using Newtonsoft.Json;
using OA.Domin.ProductsMgt.Products;
using OA.Domin.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OA.Tests.ApiIntegrationTests
{
    public class ProductsControllerTest : IntegrationTest
    {

        [Fact]
        public async Task GetAll_UnAutorized()
        {
            //Arrange
            //await Authenticate();

            //Act
            var endpoint = "api/Products";
            var response = await TestClient.GetAsync($"{endpoint}/GetAll");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        }

        [Fact]
        public async Task GetAll_NotNull()
        {
            //Arrange
            await Authenticate();

            //Act
            var endpoint = "api/Products";
            var response = await TestClient.GetAsync($"{endpoint}/GetAll");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadAsAsync<IEnumerable<Product>>();
            result.Should().NotBeNull();

        }

        [Fact]
        public async Task Create_AddOne()
        {
            //Arrange
            await Authenticate();
            var endpoint = "api/Products";
            var response1 = await TestClient.GetAsync($"{endpoint}/GetAll");
            var result1 = await response1.Content.ReadAsAsync<IEnumerable<Product>>();
            var countBefore = result1.ToList().Count();

            //Act
            Product newProduct = new Product { Name = "Test", Price = 1 };

            var response2 = await TestClient.PostAsJsonAsync($"{endpoint}", newProduct);
            var content = await response2.Content.ReadAsStringAsync();
            var result2 = JsonConvert.DeserializeObject<Response<Product>>(content);

            var response3 = await TestClient.GetAsync($"{endpoint}/GetAll");
            var result3 = await response3.Content.ReadAsAsync<IEnumerable<Product>>();
            var countAfter = result3.ToList().Count();

            //Assert
            result2.Should().NotBeNull();
            result2.HasErrors.Should().Be(false);
            result2.Result.Should().NotBeNull();
            countAfter.Should().Be(countBefore + 1);

        }

        [Fact]
        public async Task Delete_DeleteOne()
        {
            //Arrange
            await Authenticate();
            var endpoint = "api/Products";
            var response1 = await TestClient.GetAsync($"{endpoint}/GetAll");
            var result1 = await response1.Content.ReadAsAsync<IEnumerable<Product>>();
            var countBefore = result1.ToList().Count();
            var id = result1.Last().Id;

            //Act
            var response2 = await TestClient.DeleteAsync($"{endpoint}/{id}");
            var content = await response2.Content.ReadAsStringAsync();
            var result2 = JsonConvert.DeserializeObject<Response<string>>(content);

            var response3 = await TestClient.GetAsync($"{endpoint}/GetAll");
            var result3 = await response3.Content.ReadAsAsync<IEnumerable<Product>>();
            var countAfter = result3.ToList().Count();

            //Assert
            result2.Should().NotBeNull();
            result2.HasErrors.Should().Be(false);
            result2.Result.Should().Be("Deleted");
            countAfter.Should().Be(countBefore - 1);
        }


    }
}
