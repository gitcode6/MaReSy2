using MaReSy2.ConsumeModels;
using System.Diagnostics;
using System.Net.Http.Headers;
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

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return new List<SingleProduct>();  // Falls kein Token vorhanden ist, gebe eine leere Liste zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Sende die GET-Anfrage, um die SingleProducts zu erhalten
            var response = await client.GetAsync("/api/singleproducts");

            System.Diagnostics.Debug.WriteLine(response);

            var responseContent = await response.Content.ReadAsStringAsync();

            // Verarbeite die Antwort
            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<SingleProduct>>(responseContent);
            }
            else
            {
                return new List<SingleProduct>();  // Falls die Anfrage fehlschlägt, gebe eine leere Liste zurück
            }
        }


        public async Task<bool> addSingleProductAsync(CreateSingleProductModel singleProduct)
        {
            var client = _httpClientFactory.CreateClient("API");

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, gebe false zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string baseUrl = "/api/singleproducts";

            // Erstelle den Inhalt der Anfrage, indem das SingleProduct als JSON serialisiert wird
            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(singleProduct), Encoding.UTF8, "application/json");

            // Sende die POST-Anfrage
            var response = await client.PostAsync(baseUrl, stringContent);

            // Gib true zurück, wenn die Anfrage erfolgreich war, andernfalls false
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

        public async Task<bool> bearbeitenSingleProductAsync(CreateSingleProductModel singleProduct, int singleProductId)
        {
            var client = _httpClientFactory.CreateClient("API");
            string baseUrl = $"/api/singleproducts/{singleProductId}";

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, gebe false zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Erstelle die JSON-Optionen, um null-Werte zu ignorieren
            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull // Ignoriere null-Werte
            };

            // Serialisiere das SingleProduct-Objekt als JSON
            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(singleProduct, jsonOptions), Encoding.UTF8, "application/json");

            // Sende die PUT-Anfrage
            var response = await client.PutAsync(baseUrl, stringContent);

            // Gib true zurück, wenn die Anfrage erfolgreich war, andernfalls false
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

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, gebe false zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Sende die DELETE-Anfrage
            var response = await client.DeleteAsync(url);

            // Gib true zurück, wenn die Anfrage erfolgreich war, andernfalls false
            return response.IsSuccessStatusCode;
        }


    }
}
