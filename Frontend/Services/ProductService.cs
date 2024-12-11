using MaReSy2.ConsumeModels;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace MaReSy2.Services
{
    public class ProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ConsumeModels.Product>> GetProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("API");


            var response = await client.GetAsync("/api/products");

            System.Diagnostics.Debug.WriteLine(response);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<Product>>(responseContent);

            }
            else { return new List<Product>(); }

        }

        public async Task<bool> addProductAsync(Product product)
        {
            var client = _httpClientFactory.CreateClient("API");

            string baseUrl = "/api/products";


            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
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

        public async Task<bool> bearbeitenProductAsync(Product product)
        {
            var client = _httpClientFactory.CreateClient("API");
            string baseUrl = $"/api/products/{product.productId}";

            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
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

        public async Task<ConsumeModels.Product?> GetProductAsync(int productID)
        {
            var client = _httpClientFactory.CreateClient("API");

            var response = await client.GetAsync($"/api/products/{productID}");

            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(responseContent);

            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<Product>(responseContent);
            }
            else { return null; }
        }

        public async Task<bool> deleteProductAsync(int productID)
        {
            var client = _httpClientFactory.CreateClient("API");
            string url = $"/api/products/{productID}";

            var response = await client.DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }

    }
}
