using MaReSy2.ConsumeModels;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Text.Json;

namespace MaReSy2.Services
{
    public class UserService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UserService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ConsumeModels.User>> GetUsersAsync()
        {
            var client = _httpClientFactory.CreateClient("API");


            var response = await client.GetAsync("/api/users");


            var responseContent = await response.Content.ReadAsStringAsync();
            //System.Diagnostics.Debug.WriteLine(responseContent);

            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<User>>(responseContent);

            }
            else { return new List<User>(); }
        }
    }
}
