using MaReSy2.ConsumeModels;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static MaReSy2.Pages.LagerverwaltungEPHinzuModel;

namespace MaReSy2.Services
{
    public class SingleProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SingleProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ConsumeModels.SingleProduct>> GetSingleProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("API");


            var response = await client.GetAsync("/api/singleproducts");

            System.Diagnostics.Debug.WriteLine(response);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<SingleProduct>>(responseContent);

            }
            else { return new List<SingleProduct>(); }

        }

        public async Task<bool> addSingleProductAsync(CreateSingleProductModel singleProduct)

        {
            var client = _httpClientFactory.CreateClient("API");

            string baseUrl = "/api/singleproducts";


            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(singleProduct), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(baseUrl, stringContent);

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

        public async Task<bool> bearbeitenSingleProductAsync(CreateSingleProductModel singleProduct, int singleProductId)
        {
            var client = _httpClientFactory.CreateClient("API");
            string baseUrl = $"/api/singleproducts/{singleProductId}";

            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Ignoriere null-Werte
            };

            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(singleProduct), Encoding.UTF8, "application/json");
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

        public async Task<ConsumeModels.SingleProduct?> GetSingleProductAsync(int singleProductID)
            {
                var client = _httpClientFactory.CreateClient("API");

                var response = await client.GetAsync($"/api/singleproducts/{singleProductID}");

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseContent);

                if (response.IsSuccessStatusCode)
                {
                    return System.Text.Json.JsonSerializer.Deserialize<SingleProduct>(responseContent);
                }
                else { return null; }
            }

            public async Task<bool> deleteSingleProductAsync(int singleProductID)
            {
                var client = _httpClientFactory.CreateClient("API");
                string url = $"/api/singleproducts/{singleProductID}";

                var response = await client.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
        
    }
}
