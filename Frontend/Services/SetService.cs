using MaReSy2.ConsumeModels;
using System.Diagnostics;
using System.Net.Http.Headers;
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

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return new List<ConsumeModels.Set>();  // Falls kein Token vorhanden ist, gebe eine leere Liste zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Führe die GET-Anfrage aus, um die Sets abzurufen
            var response = await client.GetAsync("/api/sets");

            // Hole den Inhalt der Antwort
            var responseContent = await response.Content.ReadAsStringAsync();

            // Wenn die Anfrage erfolgreich war, deserialisiere die Antwort in eine Liste von Sets
            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<ConsumeModels.Set>>(responseContent);
            }
            else
            {
                // Wenn die Anfrage fehlschlägt, gib eine leere Liste zurück
                return new List<ConsumeModels.Set>();
            }
        }

        public async Task<ConsumeModels.Set?> GetSetAsync(int setId)
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

            // Führe die GET-Anfrage aus, um das Set mit der angegebenen setId abzurufen
            var response = await client.GetAsync($"/api/sets/{setId}");

            // Hole den Inhalt der Antwort
            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(responseContent);

            // Wenn die Anfrage erfolgreich war, deserialisiere die Antwort in ein Set-Objekt
            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<ConsumeModels.Set>(responseContent);
            }
            else
            {
                return null;  // Gib null zurück, wenn die Anfrage fehlschlägt
            }
        }

        public async Task<bool> deleteSetAsync(int setId)
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
            string url = $"/api/sets/{setId}";

            // Führe die DELETE-Anfrage aus, um das Set zu löschen
            var response = await client.DeleteAsync(url);

            // Gib true zurück, wenn die Anfrage erfolgreich war, ansonsten false
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> addSetAsync(CreateSetModel set)
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

            string baseUrl = "/api/sets";

            // Erstelle den Inhalt der Anfrage, indem das Set als JSON serialisiert wird
            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(set), Encoding.UTF8, "application/json");

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


        public async Task<bool> bearbeitenSETAsync(CreateSetModel set, int setid)
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

            string baseUrl = $"/api/sets/{setid}";

            // Erstelle den Inhalt der Anfrage, indem das Set als JSON serialisiert wird
            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(set), Encoding.UTF8, "application/json");

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

    }
}
