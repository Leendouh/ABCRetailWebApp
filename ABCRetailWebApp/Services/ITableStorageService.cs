using ABCRetailWebApp.Models;

namespace ABCRetailWebApp.Services
{
    public interface ITableStorageService
    {
        Task<Customer?> GetCustomerAsync(string customerId);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(string customerId);
        Task UpdateOrdersForCustomerAsync(string customerId, string customerName);

        Task<Product?> GetProductAsync(string productId);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(string productId);
        Task UpdateOrdersForProductAsync(string productId, string productName, double price);

        Task<Order?> GetOrderAsync(string orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(string orderId);
    }
}
