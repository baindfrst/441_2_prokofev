using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using System.Text;
using Xunit;
using GAWeb;
using System.Threading.Tasks;

namespace GAWeb.Tests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UnitTest1(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Create_Experiment_ReturnsSuccess()
        {
            var requestUri = "/api/experiments";
            var data = new
            {
                countPopulation = 5,
                countCity = 50,
                lr = 2,
                countThreads = 4
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync(requestUri, content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Start_Experiment_ReturnsSuccess()
        {
            var createResponse = await CreateExperimentHelper();
            var experimentId = JsonConvert.DeserializeObject<dynamic>(await createResponse.Content.ReadAsStringAsync()).experimentId;

            var requestUri = $"/api/experiments/{experimentId}";
            var data = new
            {
                countPopulation = 5,
                countCity = 50,
                lr = 2,
                countThreads = 4
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync(requestUri, content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task<HttpResponseMessage> CreateExperimentHelper()
        {
            var requestUri = "/api/experiments";
            var data = new
            {
                countPopulation = 5,
                countCity = 50,
                lr = 2,
                countThreads = 4
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync(requestUri, content);

            return response;
        }

        private async Task<HttpResponseMessage> StartExperimentHelper(string experimentId)
        {
            var requestUri = $"/api/experiments/{experimentId}";
            var data = new
            {
                countPopulation = 5,
                countCity = 50,
                lr = 2,
                countThreads = 4
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(data),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync(requestUri, content);

            return response;
        }
    }
}
