using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Net;

using Xunit;

namespace RaceDayApi.Tests

{
    public class ApiSmokeTests
        : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public ApiSmokeTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task SwaggerEndpoint_ReturnsSuccess()
       {           
           var response = await _client.GetAsync(
                "/swagger/v1/swagger.json");         
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task LoginEndpoint_IsAvailable()
        {
            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                new
                {
                    email = "missing.user@example.com",

                    password = "WrongPassword123!"
                });
            Assert.NotEqual(
                System.Net.HttpStatusCode.NotFound,
                response.StatusCode);
        }
    }
}
