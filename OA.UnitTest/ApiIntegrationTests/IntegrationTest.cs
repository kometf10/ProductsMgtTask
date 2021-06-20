using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using OA.DataAccess;
using OA.Domain.Requests;
using OA.Domain.Responces;
using OA_API;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OA.Tests.ApiIntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>();
                             //.WithWebHostBuilder(builder => {
                             //    builder.ConfigureServices(services =>
                             //    {
                             //        services.RemoveAll(typeof(AppDbContext));
                             //        services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDataBase() });
                             //    });
                             //});

            TestClient = appFactory.CreateClient();
        }

        protected async Task Authenticate()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetToken());
            
        }

        private async Task<string> GetToken()
        {
            var endpoint = "api/Auth";
            var loaginRequest = new LoginRequest
            {
                Email = "Admin@app.com",
                Password = "Admin@123"
            };

            var response = await TestClient.PostAsJsonAsync($"{endpoint}/Login", loaginRequest);
            var body = await response.Content.ReadAsStringAsync();

            var authResult = JsonConvert.DeserializeObject<AuthResult>(body);

            return authResult.Token;
        }
    }
}
