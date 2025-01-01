using Microsoft.AspNetCore.Mvc;

namespace MaReSy2.Controllers
{
    public class HomeController : Controller
    {
        
            // Standard-Action für die Startseite
            public IActionResult Index()
            {
                return View();
            }

            // Fehlerseite für unerwartete Fehler (z. B. 500)
            public IActionResult Error()
            {
                return View(); // Diese View wird angezeigt, wenn ein Fehler wie 500 auftritt
            }
        public IActionResult APIError()
        {
            return View(); // Diese View wird angezeigt, wenn ein API-Fehler auftritt
        }
        // Fehlerseite für 404 (Seite nicht gefunden)
        public IActionResult SeiteNichtGefunden()
            {
                return View(); // Diese View wird angezeigt, wenn ein 404-Fehler auftritt
            }
       
    }
}
