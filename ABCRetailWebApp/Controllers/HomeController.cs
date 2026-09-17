using ABCRetailWebApp.Models;
using ABCRetailWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ABCRetailWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITableStorageService _tableStorageService;

        public HomeController(ILogger<HomeController> logger, ITableStorageService tableStorageService)
        {
            _logger = logger;
            _tableStorageService = tableStorageService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var customers = await _tableStorageService.GetAllCustomersAsync();
                var products = await _tableStorageService.GetAllProductsAsync();
                var orders = await _tableStorageService.GetAllOrdersAsync();

                var dashboardData = new DashboardViewModel
                {
                    TotalCustomers = customers.Count(),
                    TotalProducts = products.Count(),
                    TotalOrders = orders.Count(),
                    PendingOrders = orders.Count(o => o.Status == "Pending"),
                    CompletedOrders = orders.Count(o => o.Status == "Delivered"),
                    TotalRevenue = orders.Where(o => o.Status != "Cancelled").Sum(o => o.TotalAmount),
                    RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList()
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                // Return empty dashboard if there's an error
                return View(new DashboardViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
