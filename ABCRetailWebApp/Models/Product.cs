using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ABCRetailWebApp.Models
{
    public class Product : ITableEntity
    {
        public string ProductId { get; set; } = string.Empty;
        
        [Display(Name = "Product Name")]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "Price (R)")]
        public double Price { get; set; }
        
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }
        
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ITableEntity implementation
        public string PartitionKey { get => ProductId; set => ProductId = value; }
        public string RowKey { get => ProductId; set => ProductId = value; }
        public DateTimeOffset? Timestamp { get; set; }
        public Azure.ETag ETag { get; set; }
    }
}
