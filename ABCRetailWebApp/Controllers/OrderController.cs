using ABCRetailWebApp.Models;
using ABCRetailWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ABCRetailWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly IQueueStorageService _queueStorageService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ITableStorageService tableStorageService, IQueueStorageService queueStorageService, ILogger<OrderController> logger)
        {
            _tableStorageService = tableStorageService;
            _queueStorageService = queueStorageService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(string id)
        {
            var order = await _tableStorageService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        public async Task<IActionResult> Create()
        {
            // Load customers and products for dropdowns
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();
            
            ViewBag.Customers = customers;
            ViewBag.Products = products;
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string customerId, string productId, int quantity, string shippingAddress)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(productId) || quantity <= 0)
            {
                // Reload dropdowns and show error
                var customers = await _tableStorageService.GetAllCustomersAsync();
                var products = await _tableStorageService.GetAllProductsAsync();
                ViewBag.Customers = customers;
                ViewBag.Products = products;
                
                ModelState.AddModelError("", "Please select a customer, product, and enter a valid quantity.");
                return View();
            }

            try
            {
                // Get customer and product details
                var customer = await _tableStorageService.GetCustomerAsync(customerId);
                var product = await _tableStorageService.GetProductAsync(productId);

                if (customer == null || product == null)
                {
                    var customers = await _tableStorageService.GetAllCustomersAsync();
                    var products = await _tableStorageService.GetAllProductsAsync();
                    ViewBag.Customers = customers;
                    ViewBag.Products = products;
                    
                    ModelState.AddModelError("", "Invalid customer or product selected.");
                    return View();
                }

                // Auto-generate Order ID: ORD-yyyyMMdd-XXXX
                var orderId = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
                
                // Calculate total amount
                var totalAmount = product.Price * quantity;

                // Create order with auto-generated fields
                var order = new Order
                {
                    OrderId = orderId,
                    CustomerId = customerId,
                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                    ProductId = productId,
                    ProductName = product.Name,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    ShippingAddress = string.IsNullOrEmpty(shippingAddress) ? customer.Address : shippingAddress,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = productId,
                            ProductName = product.Name,
                            Quantity = quantity,
                            UnitPrice = product.Price
                        }
                    }
                };

                await _tableStorageService.AddOrderAsync(order);
                
                // Send order notification to queue
                var orderMessage = JsonSerializer.Serialize(new
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    CustomerName = order.CustomerName,
                    ProductName = product.Name,
                    Quantity = quantity,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    OrderDate = order.OrderDate
                });
                await _queueStorageService.SendMessageAsync("order-transactions", orderMessage);
                
                TempData["SuccessMessage"] = $"Order {orderId} created successfully for {customer.FirstName} {customer.LastName}!";
                _logger.LogInformation($"Order {order.OrderId} created and queued for processing");
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Reload dropdowns on error
                var customers = await _tableStorageService.GetAllCustomersAsync();
                var products = await _tableStorageService.GetAllProductsAsync();
                ViewBag.Customers = customers;
                ViewBag.Products = products;
                
                ModelState.AddModelError("", $"Error creating order: {ex.Message}");
                return View();
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            var order = await _tableStorageService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            
            // Load customers and products for dropdowns
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();
            
            ViewBag.Customers = customers;
            ViewBag.Products = products;
            
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string customerId, string productId, int quantity, string orderStatus, string shippingAddress)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(productId) || quantity <= 0)
            {
                // Reload dropdowns and show error
                var customers = await _tableStorageService.GetAllCustomersAsync();
                var products = await _tableStorageService.GetAllProductsAsync();
                ViewBag.Customers = customers;
                ViewBag.Products = products;
                
                // Get the original order to display
                var order = await _tableStorageService.GetOrderAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                
                ModelState.AddModelError("", "Please select a customer, product, and enter a valid quantity.");
                return View(order);
            }

            try
            {
                // Get customer and product details
                var customer = await _tableStorageService.GetCustomerAsync(customerId);
                var product = await _tableStorageService.GetProductAsync(productId);

                if (customer == null || product == null)
                {
                    var customers = await _tableStorageService.GetAllCustomersAsync();
                    var products = await _tableStorageService.GetAllProductsAsync();
                    ViewBag.Customers = customers;
                    ViewBag.Products = products;
                    
                    var order = await _tableStorageService.GetOrderAsync(id);
                    if (order == null)
                    {
                        return NotFound();
                    }
                    
                    ModelState.AddModelError("", "Invalid customer or product selected.");
                    return View(order);
                }

                // Get the existing order
                var existingOrder = await _tableStorageService.GetOrderAsync(id);
                if (existingOrder == null)
                {
                    return NotFound();
                }

                // Calculate total amount
                var totalAmount = product.Price * quantity;

                // Update order with new values
                existingOrder.CustomerId = customerId;
                existingOrder.CustomerName = $"{customer.FirstName} {customer.LastName}";
                existingOrder.ProductId = productId;
                existingOrder.ProductName = product.Name;
                existingOrder.Quantity = quantity;
                existingOrder.UnitPrice = product.Price;
                existingOrder.TotalAmount = totalAmount;
                existingOrder.Status = string.IsNullOrEmpty(orderStatus) ? "Pending" : orderStatus;
                existingOrder.ShippingAddress = string.IsNullOrEmpty(shippingAddress) ? customer.Address : shippingAddress;
                
                // Update OrderItems
                existingOrder.OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = productId,
                        ProductName = product.Name,
                        Quantity = quantity,
                        UnitPrice = product.Price
                    }
                };

                await _tableStorageService.UpdateOrderAsync(existingOrder);
                
                // Send order update notification to queue
                var orderMessage = JsonSerializer.Serialize(new
                {
                    OrderId = existingOrder.OrderId,
                    CustomerId = existingOrder.CustomerId,
                    CustomerName = existingOrder.CustomerName,
                    ProductName = existingOrder.ProductName,
                    Quantity = existingOrder.Quantity,
                    TotalAmount = existingOrder.TotalAmount,
                    Status = orderStatus,
                    OrderDate = existingOrder.OrderDate,
                    Message = "Order updated"
                });
                await _queueStorageService.SendMessageAsync("order-transactions", orderMessage);
                
                TempData["SuccessMessage"] = $"Order {existingOrder.OrderId} updated successfully!";
                _logger.LogInformation($"Order {existingOrder.OrderId} updated and queued for processing with status: {orderStatus}");
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Reload dropdowns on error
                var customers = await _tableStorageService.GetAllCustomersAsync();
                var products = await _tableStorageService.GetAllProductsAsync();
                ViewBag.Customers = customers;
                ViewBag.Products = products;
                
                var order = await _tableStorageService.GetOrderAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                
                ModelState.AddModelError("", $"Error updating order: {ex.Message}");
                return View(order);
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            var order = await _tableStorageService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var order = await _tableStorageService.GetOrderAsync(id);
            if (order != null)
            {
                await _tableStorageService.DeleteOrderAsync(id);
                _logger.LogInformation($"Order {order.OrderId} deleted");
                TempData["SuccessMessage"] = $"Order {order.OrderId} deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
