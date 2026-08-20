namespace SolarVolt.DTOs
{
    public class DashboardStatsDto
    {
        // 1️⃣ الكروت العلوية (Cards)
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public double AverageUserLoad { get; set; }
        public int LowStockProductsCount { get; set; }

        // 2️⃣ مخطط المبيعات (Sales Chart)
        public List<SalesChartPointDto> SalesOverview { get; set; } = new();

        // 3️⃣ الأكثر مبيعاً حسب الفئة (Category Pie Chart)
        public List<CategorySalesDto> SalesByCategory { get; set; } = new();

        // 4️⃣ آخر 5 طلبات (Recent Orders Table)
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    public class SalesChartPointDto
    {
        public string Date { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class CategorySalesDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public double Percentage { get; set; }
    }

    public class RecentOrderDto
    {
        public int OrderId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}