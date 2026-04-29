using Microsoft.AspNetCore.Mvc;
using StoreMongoApp.Models;
using StoreMongoApp.Services;

namespace StoreMongoApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _productService.GetAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(string id)
        {
            Product? product = await _productService.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _productService.CreateAsync(product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            Product? product = await _productService.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Product product)
        {
            product.Id = id;
            await _productService.UpdateAsync(id, product);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            Product? product = await _productService.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _productService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}