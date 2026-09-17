namespace ABCRetailWebApp.Models
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public double TotalRevenue { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}