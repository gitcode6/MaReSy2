using MaReSy2.ConsumeModels;
using System.Diagnostics;
using System.Text;

namespace MaReSy2.Services
{
    public class SetService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SetService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ConsumeModels.Set>> GetSetsAsync()
        {
            var client = _httpClientFactory.CreateClient("API");

            var response = await client.GetAsync("/api/sets");

            System.Diagnostics.Debug.WriteLine(response);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<Set>>(responseContent);
            }
            else { return new List<Set>(); }
        }
        public async Task<ConsumeModels.Set?> GetSetAsync(int setId)
        {
            var client = _httpClientFactory.CreateClient("API");

            var response = await client.GetAsync($"/api/sets/{setId}");

            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(responseContent);

            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<Set>(responseContent);
            }
            else { return null; }
        }

        public async Task<bool> deleteSetAsync(int setId)
        {
            var client = _httpClientFactory.CreateClient("API");
            string url = $"/api/sets/{setId}";

            var response = await client.DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> addSetAsync(CreateSetModel set)
        {
            var client = _httpClientFactory.CreateClient("API");

            string baseUrl = "/api/sets";


            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(set), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response.Content.ReadAsStringAsync());
                return true;
            }
            else
            {
                Debug.WriteLine(response.Content.ReadAsStringAsync());
                return false;
            }

        }

        public async Task<bool> bearbeitenSETAsync(CreateSetModel set, int setid)
        {
            var client = _httpClientFactory.CreateClient("API");
            string baseUrl = $"/api/sets/{setid}";

            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(set), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(baseUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(response.Content.ReadAsStringAsync());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
