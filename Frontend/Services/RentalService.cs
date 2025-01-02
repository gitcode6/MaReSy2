using MaReSy2.ConsumeModels;
using Newtonsoft.Json;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace MaReSy2.Services
{
    public class RentalService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RentalService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<ConsumeModels.Rental>> GetRentalsAsync()
        {
            var client = _httpClientFactory.CreateClient("API");

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return new List<ConsumeModels.Rental>();  // Falls kein Token vorhanden ist, gib eine leere Liste zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Führe die GET-Anfrage aus, um die Produkte abzurufen
            var response = await client.GetAsync("/api/rentals");

            // Logge die Antwort für Debugging-Zwecke
            System.Diagnostics.Debug.WriteLine(response);

            // Lese den Antwortinhalt als String
            var responseContent = await response.Content.ReadAsStringAsync();

            // Wenn die Anfrage erfolgreich war, deserialisiere die Antwort in eine Liste von Produkten
            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<ConsumeModels.Rental>>(responseContent);
            }
            else
            {
                // Gib eine leere Liste zurück, wenn die Anfrage fehlgeschlagen ist
                return new List<ConsumeModels.Rental>();
            }
        }

        public async Task<List<ConsumeModels.Rental>> GetUserRentalsAsync()
        {
            var client = _httpClientFactory.CreateClient("API");

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return new List<ConsumeModels.Rental>();  // Falls kein Token vorhanden ist, gib eine leere Liste zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Führe die GET-Anfrage aus, um die Produkte abzurufen
            var response = await client.GetAsync("/api/rentals/userrentals");

            // Logge die Antwort für Debugging-Zwecke
            System.Diagnostics.Debug.WriteLine(response);

            // Lese den Antwortinhalt als String
            var responseContent = await response.Content.ReadAsStringAsync();

            // Wenn die Anfrage erfolgreich war, deserialisiere die Antwort in eine Liste von Produkten
            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<ConsumeModels.Rental>>(responseContent);
            }
            else
            {
                // Gib eine leere Liste zurück, wenn die Anfrage fehlgeschlagen ist
                return new List<ConsumeModels.Rental>();
            }
        }

        public async Task<bool> makeRentalAsync(MakeRental rental)
        {
            var client = _httpClientFactory.CreateClient("API");

            string baseUrl = "/api/rentals";

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, gib false zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Erstelle den StringContent mit dem Produkt-Objekt
            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(rental), Encoding.UTF8, "application/json");

            // Führe die POST-Anfrage aus, um das Produkt hinzuzufügen
            var response = await client.PostAsync(baseUrl, stringContent);

            // Wenn die Anfrage erfolgreich war, gib true zurück, ansonsten false
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(await response.Content.ReadAsStringAsync());
                return true;
            }
            else
            {
                Debug.WriteLine("Fehler beim Hinzufügen des Produkts: " + response.StatusCode);
                return false;
            }
        }


        public async Task<bool> bearbeitenProductAsync(Product product)
        {
            var client = _httpClientFactory.CreateClient("API");

            // Setze die URL, um das Produkt zu bearbeiten
            string baseUrl = $"/api/products/{product.productId}";

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, gebe false zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Erstelle den StringContent mit dem Produkt-Objekt
            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");

            // Führe die PUT-Anfrage aus, um das Produkt zu bearbeiten
            var response = await client.PutAsync(baseUrl, stringContent);

            // Wenn die Anfrage erfolgreich war, gib true zurück, ansonsten false
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(await response.Content.ReadAsStringAsync());
                return true;
            }
            else
            {
                Debug.WriteLine("Fehler beim Bearbeiten des Produkts: " + response.StatusCode);
                return false;
            }
        }

        public async Task<ConsumeModels.Rental?> GetRentalAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("API");

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return null;  // Falls kein Token vorhanden ist, gib null zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Führe die GET-Anfrage aus, um das Produkt abzurufen
            var response = await client.GetAsync($"/api/rentals/{id}");

            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(responseContent);

            // Wenn die Anfrage erfolgreich war, deserialisiere die Antwort in das Product-Objekt
            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<ConsumeModels.Rental>(responseContent);
            }
            else
            {
                return null;  // Gib null zurück, wenn die Anfrage fehlschlägt
            }
        }


        public async Task<bool> cancelRentalAsync(int id)
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

            // Erstelle die URL für die DELETE-Anfrage
            string url = $"/api/rentals/{id}/cancel";

            // Führe die DELETE-Anfrage aus, um das Produkt zu löschen
            var response = await client.DeleteAsync(url);

            // Gib true zurück, wenn die Anfrage erfolgreich war, ansonsten false
            return response.IsSuccessStatusCode;
        }

        public async Task<byte[]?> GetProductImageAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("API");

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return null;  // Falls kein Token vorhanden ist, gebe false zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Erstelle die URL für die DELETE-Anfrage
            string url = $"/api/products/{id}/image";

            // Führe die DELETE-Anfrage aus, um das Produkt zu löschen
            var response = await client.GetAsync(url);

            // Wenn die Anfrage erfolgreich war, deserialisiere die Antwort in das Product-Objekt
            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                return imageBytes;
            }
            else
            {
                return null;
            }
        }


    }
}
