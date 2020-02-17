using EmployeeApi;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeAPI.Tests
{
    public class EmployeeControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private HttpClient Client;

        public EmployeeControllerTests(TestFixture<Startup> fixture)
        {
            Client = fixture.Client;
        }

        [Fact]
        public async Task EmployeeListGetTestAsync()
        {
            // Arrange
            var request = "/api/Employee";

            // Act
            var response = await Client.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task EmployeeCreateTestAsync()
        {
            // Arrange
            var request = new
            {
                Url = "/api/Employee",
                Body = new
                {
                    FirstName ="Api",
                    LastName = "test",
                    EmailAddress = "apitest@gmail.com"
                }
            };

            // Act
            var response = await Client.PostAsync(request.Url, GetStringContent(request.Body));
            var value = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
        }

        public static StringContent GetStringContent(object obj)
           => new StringContent(JsonSerializer.Serialize(obj), Encoding.Default, "application/json");
    }
}
