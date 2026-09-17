using Azure;
using Azure.Data.Tables;
using ABCRetailWebApp.Models;
using System.Text.Json;

namespace ABCRetailWebApp.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly TableClient _customerTableClient;
        private readonly TableClient _productTableClient;
        private readonly TableClient _orderTableClient;

        public TableStorageService(string connectionString)
        {
            var serviceClient = new TableServiceClient(connectionString);
            _customerTableClient = serviceClient.GetTableClient("Customers");
            _productTableClient = serviceClient.GetTableClient("Products");
            _orderTableClient = serviceClient.GetTableClient("Orders");

            _customerTableClient.CreateIfNotExists();
            _productTableClient.CreateIfNotExists();
            _orderTableClient.CreateIfNotExists();
        }

        public async Task<Customer?> GetCustomerAsync(string customerId)
        {
            try
            {
                var response = await _customerTableClient.GetEntityAsync<Customer>(customerId, customerId);
                return response.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();
            await foreach (var customer in _customerTableClient.QueryAsync<Customer>())
            {
                customers.Add(customer);
            }
            return customers;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            customer.CustomerId = customer.CustomerId == string.Empty ? Guid.NewGuid().ToString() : customer.CustomerId;
            await _customerTableClient.AddEntityAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerTableClient.UpdateEntityAsync(customer, Azure.ETag.All);
        }

        public async Task DeleteCustomerAsync(string customerId)
        {
            await _customerTableClient.DeleteEntityAsync(customerId, customerId);
        }

        public async Task UpdateOrdersForCustomerAsync(string customerId, string customerName)
        {
            // Find all orders for this customer
            var ordersForCustomer = new List<Order>();
            await foreach (var order in _orderTableClient.QueryAsync<Order>(o => o.CustomerId == customerId))
            {
                ordersForCustomer.Add(order);
            }

            // Update each order with the new customer name
            foreach (var order in ordersForCustomer)
            {
                order.CustomerName = customerName;
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                order.OrderItemsJson = System.Text.Json.JsonSerializer.Serialize(order.OrderItems, options);
                await _orderTableClient.UpdateEntityAsync(order, Azure.ETag.All);
            }
        }

        public async Task<Product?> GetProductAsync(string productId)
        {
            try
            {
                var response = await _productTableClient.GetEntityAsync<Product>(productId, productId);
                return response.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in _productTableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        public async Task AddProductAsync(Product product)
        {
            product.ProductId = product.ProductId == string.Empty ? Guid.NewGuid().ToString() : product.ProductId;
            await _productTableClient.AddEntityAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productTableClient.UpdateEntityAsync(product, Azure.ETag.All);
        }

        public async Task DeleteProductAsync(string productId)
        {
            await _productTableClient.DeleteEntityAsync(productId, productId);
        }

        public async Task UpdateOrdersForProductAsync(string productId, string productName, double price)
        {
            // Find all orders for this product
            var ordersForProduct = new List<Order>();
            await foreach (var order in _orderTableClient.QueryAsync<Order>(o => o.ProductId == productId))
            {
                ordersForProduct.Add(order);
            }

            // Update each order with the new product name and price
            foreach (var order in ordersForProduct)
            {
                order.ProductName = productName;
                order.UnitPrice = price;
                order.TotalAmount = order.Quantity * price;
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                order.OrderItemsJson = System.Text.Json.JsonSerializer.Serialize(order.OrderItems, options);
                await _orderTableClient.UpdateEntityAsync(order, Azure.ETag.All);
            }
        }

        public async Task<Order?> GetOrderAsync(string orderId)
        {
            try
            {
                var response = await _orderTableClient.GetEntityAsync<Order>(orderId, orderId);
                var order = response.Value;
                // Deserialize order items from JSON with proper options
                if (!string.IsNullOrEmpty(order.OrderItemsJson))
                {
                    try
                    {
                        var options = new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                        };
                        order.OrderItems = System.Text.Json.JsonSerializer.Deserialize<List<OrderItem>>(order.OrderItemsJson, options) ?? new List<OrderItem>();
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // If deserialization fails, create empty list
                        order.OrderItems = new List<OrderItem>();
                    }
                }
                return order;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();
            await foreach (var order in _orderTableClient.QueryAsync<Order>())
            {
                // Deserialize order items from JSON with proper options
                if (!string.IsNullOrEmpty(order.OrderItemsJson))
                {
                    try
                    {
                        var options = new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                        };
                        order.OrderItems = System.Text.Json.JsonSerializer.Deserialize<List<OrderItem>>(order.OrderItemsJson, options) ?? new List<OrderItem>();
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // If deserialization fails, create empty list
                        order.OrderItems = new List<OrderItem>();
                    }
                }
                orders.Add(order);
            }
            return orders;
        }

        public async Task AddOrderAsync(Order order)
        {
            order.OrderId = order.OrderId == string.Empty ? Guid.NewGuid().ToString() : order.OrderId;
            // Serialize order items to JSON for storage with proper options
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            order.OrderItemsJson = System.Text.Json.JsonSerializer.Serialize(order.OrderItems, options);
            await _orderTableClient.AddEntityAsync(order);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            // Serialize order items to JSON for storage with proper options
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            order.OrderItemsJson = System.Text.Json.JsonSerializer.Serialize(order.OrderItems, options);
            await _orderTableClient.UpdateEntityAsync(order, Azure.ETag.All);
        }

        public async Task DeleteOrderAsync(string orderId)
        {
            await _orderTableClient.DeleteEntityAsync(orderId, orderId);
        }
    }
}
