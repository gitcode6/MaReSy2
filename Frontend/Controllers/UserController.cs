using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MaReSy2.Controllers
{
    public class UserController : Controller
    {
        Uri baseAdress = new Uri("https://localhost:7162/api");
        private readonly HttpClient _client;

        public UserController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAdress;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var userList = new List<ConsumeModels.User>();

            HttpResponseMessage response = await _client.GetAsync(baseAdress + "/users");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                userList = JsonConvert.DeserializeObject<List<ConsumeModels.User>>(data);
            }

            return View(userList);
        }
    }
}
