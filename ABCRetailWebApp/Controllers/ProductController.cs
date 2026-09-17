using ABCRetailWebApp.Models;
using ABCRetailWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ITableStorageService tableStorageService, IBlobStorageService blobStorageService, ILogger<ProductController> logger)
        {
            _tableStorageService = tableStorageService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(string id)
        {
            var product = await _tableStorageService.GetProductAsync(id);
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                product.ProductId = product.ProductId == string.Empty ? Guid.NewGuid().ToString() : product.ProductId;
                
                // Handle image upload to blob storage
                if (imageFile != null && imageFile.Length > 0)
                {
                    var blobName = $"{product.ProductId}_{imageFile.FileName}";
                    using var stream = imageFile.OpenReadStream();
                    product.ImageUrl = await _blobStorageService.UploadBlobAsync("product-images", blobName, stream, imageFile.ContentType);
                }

                await _tableStorageService.AddProductAsync(product);
                _logger.LogInformation($"Product {product.ProductId} created");
                TempData["SuccessMessage"] = $"Product {product.Name} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var product = await _tableStorageService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            // Preserve the existing ImageUrl
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Product product, IFormFile? imageFile)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Get the original product to check if name or price changed
                var existingProduct = await _tableStorageService.GetProductAsync(id);
                bool nameChanged = existingProduct != null && existingProduct.Name != product.Name;
                bool priceChanged = existingProduct != null && existingProduct.Price != product.Price;
                
                if (existingProduct != null)
                {
                    // Handle image upload to blob storage only if new image is provided
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var blobName = $"{product.ProductId}_{imageFile.FileName}";
                        using var stream = imageFile.OpenReadStream();
                        product.ImageUrl = await _blobStorageService.UploadBlobAsync("product-images", blobName, stream, imageFile.ContentType);
                    }
                    else
                    {
                        // Keep the existing ImageUrl if no new image is provided
                        product.ImageUrl = existingProduct.ImageUrl;
                    }
                }

                await _tableStorageService.UpdateProductAsync(product);
                _logger.LogInformation($"Product {product.ProductId} updated");
                
                // Update all orders for this product if name or price changed
                if (nameChanged || priceChanged)
                {
                    await _tableStorageService.UpdateOrdersForProductAsync(product.ProductId, product.Name, product.Price);
                    _logger.LogInformation($"Updated orders for product {product.ProductId} with new name: {product.Name} and price: {product.Price}");
                }
                
                TempData["SuccessMessage"] = $"Product {product.Name} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var product = await _tableStorageService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _tableStorageService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
