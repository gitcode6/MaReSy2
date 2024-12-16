using MaReSy2.ConsumeModels;

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
    }
}
