using ABCRetailWebApp.Models;
using ABCRetailWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailWebApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ITableStorageService _tableStorageService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ITableStorageService tableStorageService, ILogger<CustomerController> logger)
        {
            _tableStorageService = tableStorageService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            return View(customers);
        }

        public async Task<IActionResult> Details(string id)
        {
            var customer = await _tableStorageService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CustomerId = customer.CustomerId == string.Empty ? Guid.NewGuid().ToString() : customer.CustomerId;
                await _tableStorageService.AddCustomerAsync(customer);
                _logger.LogInformation($"Customer {customer.CustomerId} created");
                TempData["SuccessMessage"] = $"Customer {customer.FirstName} {customer.LastName} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var customer = await _tableStorageService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Get the original customer to check if name changed
                var originalCustomer = await _tableStorageService.GetCustomerAsync(id);
                bool nameChanged = originalCustomer != null && 
                    (originalCustomer.FirstName != customer.FirstName || originalCustomer.LastName != customer.LastName);
                
                await _tableStorageService.UpdateCustomerAsync(customer);
                _logger.LogInformation($"Customer {customer.CustomerId} updated");
                
                // Update all orders for this customer if name changed
                if (nameChanged)
                {
                    string fullName = $"{customer.FirstName} {customer.LastName}";
                    await _tableStorageService.UpdateOrdersForCustomerAsync(customer.CustomerId, fullName);
                    _logger.LogInformation($"Updated orders for customer {customer.CustomerId} with new name: {fullName}");
                }
                
                TempData["SuccessMessage"] = $"Customer {customer.FirstName} {customer.LastName} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var customer = await _tableStorageService.GetCustomerAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _tableStorageService.GetCustomerAsync(id);
            if (customer != null)
            {
                await _tableStorageService.DeleteCustomerAsync(id);
                _logger.LogInformation($"Customer {id} deleted");
                TempData["SuccessMessage"] = $"Customer {customer.FirstName} {customer.LastName} deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
