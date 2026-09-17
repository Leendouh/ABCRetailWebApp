using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ABCRetailWebApp.Models
{
    public class Customer : ITableEntity
    {
        public string CustomerId { get; set; } = string.Empty;
        
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;
        
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
        
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ITableEntity implementation
        public string PartitionKey { get => CustomerId; set => CustomerId = value; }
        public string RowKey { get => CustomerId; set => CustomerId = value; }
        public DateTimeOffset? Timestamp { get; set; }
        public Azure.ETag ETag { get; set; }
    }
}
