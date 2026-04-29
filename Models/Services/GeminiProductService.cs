using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using StoreMongoApp.Models;

namespace StoreMongoApp.Services
{
    public class GeminiProductService
    {
        private readonly ProductService _productService;
        private readonly GeminiSettings _geminiSettings;
        private readonly HttpClient _httpClient;

        public GeminiProductService(
            ProductService productService,
            IOptions<GeminiSettings> geminiSettings,
            HttpClient httpClient)
        {
            _productService = productService;
            _geminiSettings = geminiSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<string> AskAboutProductsAsync(string question)
        {
            List<Product> products = await _productService.GetAsync();

            string productJson = JsonSerializer.Serialize(products);

            string prompt = $@"
You are an AI shopping assistant for an online store.
Use only the product data below to answer the user's question.

Product Data:
{productJson}

User Question:
{question}

Answer clearly and naturally. If the answer cannot be found in the product data, say that.
";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            string url =
                $"https://generativelanguage.googleapis.com/v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";

            string jsonBody = JsonSerializer.Serialize(requestBody);
            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                string fallbackAnswer = BuildFallbackAnswer(question, products);

                return $@"
Gemini API Response:
The request reached Gemini, but Gemini could not generate a final response because of an API quota or configuration issue.

Database-Based Fallback Response:
{fallbackAnswer}

This fallback response was generated from the MongoDB Products collection that was retrieved by ProductService.
";
            }

            string responseJson = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(responseJson);

            string? answer = document
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return answer ?? "No response was generated.";
        }

        private string BuildFallbackAnswer(string question, List<Product> products)
        {
            string lowerQuestion = question.ToLower();

            if (products.Count == 0)
            {
                return "No products were found in the MongoDB Products collection.";
            }

            if (lowerQuestion.Contains("cheapest"))
            {
                var cheapestProducts = products
                    .OrderBy(p => p.Price)
                    .Take(3)
                    .Select(p => $"{p.Name} costs ${p.Price} and is in the {p.Category} category.");

                return "The cheapest products are: " + string.Join(" ", cheapestProducts);
            }

            if (lowerQuestion.Contains("stock") || lowerQuestion.Contains("in stock"))
            {
                var inStockProducts = products
                    .Where(p => p.InStock)
                    .Select(p => $"{p.Name} costs ${p.Price} and is currently in stock.");

                return "The products currently in stock are: " + string.Join(" ", inStockProducts);
            }

            if (lowerQuestion.Contains("electronics"))
            {
                var electronics = products
                    .Where(p => p.Category.ToLower().Contains("electronics"))
                    .Select(p => $"{p.Name} costs ${p.Price}.");

                return "The electronics products are: " + string.Join(" ", electronics);
            }

            var allProducts = products
                .Select(p => $"{p.Name} costs ${p.Price}, is in the {p.Category} category, and in-stock status is {p.InStock}.");

            return "Here are the products from MongoDB: " + string.Join(" ", allProducts);
        }
    }
}