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


            var response = await client.GetAsync("/api/users");


            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(responseContent);

            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<User>>(responseContent);

            }
            else { return new List<User>(); }
        }

        public async Task<bool> addUserAsync(User user)
        {
            var client = _httpClientFactory.CreateClient("API");

            string baseUrl = "/api/users";

            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

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

        public async Task<bool> deleteUserAsync(int userId)
        {
            var client = _httpClientFactory.CreateClient("API");
            string url = $"/api/users/{userId}";

            var response  = await client.DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> bearbeitenUserAsync(User user)
        {
            var client = _httpClientFactory.CreateClient("API");
            string baseUrl = $"/api/users/{user.userId}";

            using StringContent stringContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
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

        public async Task<ConsumeModels.User?> GetUserAsync(int userID)
        {
            var client = _httpClientFactory.CreateClient("API");


            var response = await client.GetAsync($"/api/users/{userID}");

            var responseContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(responseContent);

            if (response.IsSuccessStatusCode)
            {
                return System.Text.Json.JsonSerializer.Deserialize<User>(responseContent);

            }
            else { return null;  }
        }

        public class LoginResponse
        {
            public User User { get; set; }
            public string Token { get; set; }
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
                    string token = loginResponse.Token;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

                    // Gib den User zurück
                    return loginResponse.User;
                }
            }
            else
            {
                Debug.WriteLine("Login failed with status code: " + response.StatusCode);
                return null;
            }

            return null;
        }

    }
}
