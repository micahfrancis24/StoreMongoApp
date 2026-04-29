using Microsoft.AspNetCore.Mvc;
using StoreMongoApp.Services;

namespace StoreMongoApp.Controllers
{
    public class AIController : Controller
    {
        private readonly GeminiProductService _geminiProductService;

        public AIController(GeminiProductService geminiProductService)
        {
            _geminiProductService = geminiProductService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ask(string question)
        {
            string response = await _geminiProductService.AskAboutProductsAsync(question);

            ViewBag.Question = question;
            ViewBag.Response = response;

            return View("Index");
        }
    }
}