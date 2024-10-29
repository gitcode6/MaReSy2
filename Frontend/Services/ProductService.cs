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

        public async Task<bool> addProductAsync(string productname, string productdescription, bool productstatus, int productamount)
        {
            var client = _httpClientFactory.CreateClient("API");

            string baseUrl = "/api/products";
            string requestUrl = $"{baseUrl}?Productname={productname}&Productdescription={productdescription}&Productactive={productstatus}&Productamount={productamount}";

            var response = await client.PostAsync(requestUrl, null);
            


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
