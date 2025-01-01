using MaReSy2.ConsumeModels;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
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

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return new List<ConsumeModels.User>();  // Keine Daten, wenn der Token fehlt
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/users");

            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(responseContent);

            if (response.IsSuccessStatusCode)
            {
                // Deserialisieren der Antwort in die Liste der User-Objekte
                return System.Text.Json.JsonSerializer.Deserialize<List<ConsumeModels.User>>(responseContent);
            }
            else
            {
                return new List<ConsumeModels.User>();  // Leere Liste zurückgeben, wenn die Anfrage fehlschlägt
            }
        }


        public async Task<bool> addUserAsync(User user)
        {
            var client = _httpClientFactory.CreateClient("API");

            string baseUrl = "/api/users";

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, breche die Anfrage ab
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                // Erfolgreiche Antwort
                Debug.WriteLine(await response.Content.ReadAsStringAsync());
                return true;
            }
            else
            {
                // Fehlerhafte Antwort
                Debug.WriteLine("Fehler beim Erstellen des Benutzers: " + response.StatusCode);
                return false;
            }
        }

        public async Task<bool> deleteUserAsync(int userId)
        {
            var client = _httpClientFactory.CreateClient("API");

            // Setze die URL, um den User zu löschen
            string url = $"/api/users/{userId}";

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, breche die Anfrage ab
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Führe die DELETE-Anfrage aus
            var response = await client.DeleteAsync(url);

            // Gib true zurück, wenn die Antwort erfolgreich war
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> bearbeitenUserAsync(User user)
        {
            var client = _httpClientFactory.CreateClient("API");

            // Setze die URL, um den Benutzer zu bearbeiten
            string baseUrl = $"/api/users/{user.userId}";

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, breche die Anfrage ab
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Erstelle den StringContent mit dem zu sendenden User-Objekt
            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            // Führe die PUT-Anfrage aus
            var response = await client.PutAsync(baseUrl, stringContent);

            // Gib true zurück, wenn die Antwort erfolgreich war
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(await response.Content.ReadAsStringAsync());
                return true;
            }
            else
            {
                // Gib false zurück, wenn ein Fehler aufgetreten ist
                Debug.WriteLine("Fehler beim Bearbeiten des Benutzers: " + response.StatusCode);
                return false;
            }
        }


        public async Task<ConsumeModels.User?> GetUserAsync(int userID)
        {
            var client = _httpClientFactory.CreateClient("API");

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return null;  // Falls kein Token vorhanden ist, gebe null zurück
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Führe die GET-Anfrage aus
            var response = await client.GetAsync($"/api/users/{userID}");

            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(responseContent);

            // Wenn die Anfrage erfolgreich war, deserialisiere die Antwort in das User-Objekt
            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<ConsumeModels.User>(responseContent);
            }
            else
            {
                return null;  // Gib null zurück, wenn die Anfrage fehlschlägt
            }
        }


        public class LoginResponse
        {
            public User user { get; set; }
            public string token { get; set; }
        }


        public async Task<ConsumeModels.User?> GetLoginAsync(User meinUser)
        {
            var client = _httpClientFactory.CreateClient("API");

            string baseUrl = "/api/auth/login";

            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(meinUser), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(baseUrl, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseContent);

                // Deserialisiere die Antwort in das LoginResponse-Objekt
                var loginResponse = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseContent);

                if (loginResponse != null)
                {
                    // Speichere das Token, falls notwendig (z.B. im Header für zukünftige Anfragen)
                    string token = loginResponse.token;
                    TokenManager.SetToken(loginResponse.token);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.token);

                    UserManager.SetUser(loginResponse.user);

                    // Gib den User zurück
                    return loginResponse.user;
                }
            }
            else
            {
                Debug.WriteLine("Login failed with status code: " + response.StatusCode);
                return null;
            }

            return null;
        }

        public async Task<bool> passwordBearbeiten(User angemeldeterUser)
        {
            var client = _httpClientFactory.CreateClient("API");

            // Setze die URL, um den Benutzer zu bearbeiten
            string baseUrl = $"/api/users/me/change-password";

            // Hole den Token (angenommen, du hast ihn irgendwo gespeichert, z.B. im TokenManager)
            string token = TokenManager.GetToken();  // Beispiel für eine Klasse, die den Token speichert

            if (string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("Kein Token gefunden. Bitte zuerst anmelden.");
                return false;  // Falls kein Token vorhanden ist, breche die Anfrage ab
            }

            // Setze den Authorization-Header mit dem Bearer-Token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Erstelle den StringContent mit dem zu sendenden User-Objekt
            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(angemeldeterUser), Encoding.UTF8, "application/json");

            // Führe die PUT-Anfrage aus
            var response = await client.PutAsync(baseUrl, stringContent);

            // Gib true zurück, wenn die Antwort erfolgreich war
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine(await response.Content.ReadAsStringAsync());
                return true;
            }
            else
            {
                // Gib false zurück, wenn ein Fehler aufgetreten ist
                Debug.WriteLine("Fehler beim Bearbeiten des Benutzers: " + response.StatusCode);
                return false;
            }
        }

    }
}
