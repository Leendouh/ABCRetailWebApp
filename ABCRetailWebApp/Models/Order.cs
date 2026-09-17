using Azure.Data.Tables;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ABCRetailWebApp.Models
{
    public class Order : ITableEntity
    {
        public string OrderId { get; set; } = string.Empty;
        
        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; } = string.Empty;
        
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;    // denormalized for display
        
        [Display(Name = "Product ID")]
        public string ProductId { get; set; } = string.Empty;
        
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;     // denormalized for display
        
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Unit Price (R)")]
        public double UnitPrice { get; set; }
        
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        
        [Display(Name = "Total Amount (R)")]
        public double TotalAmount { get; set; }
        
        [Display(Name = "Order Status")]
        public string Status { get; set; } = "Pending";
        
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;
        
        // Store order items as JSON string for Table Storage
        public string OrderItemsJson { get; set; } = string.Empty;
        
        // This property is for in-memory use only and is ignored by Azure SDK
        [JsonIgnore]
        [IgnoreDataMember]
        public List<OrderItem> OrderItems 
        { 
            get
            {
                if (string.IsNullOrEmpty(OrderItemsJson))
                    return new List<OrderItem>();
                try
                {
                    return JsonSerializer.Deserialize<List<OrderItem>>(OrderItemsJson) ?? new List<OrderItem>();
                }
                catch
                {
                    return new List<OrderItem>();
                }
            }
            set
            {
                OrderItemsJson = JsonSerializer.Serialize(value);
            }
        }

        // ITableEntity implementation
        public string PartitionKey { get => OrderId; set => OrderId = value; }
        public string RowKey { get => OrderId; set => OrderId = value; }
        public DateTimeOffset? Timestamp { get; set; }
        public Azure.ETag ETag { get; set; }
    }

    public class OrderItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice => Quantity * UnitPrice;
    }
}
