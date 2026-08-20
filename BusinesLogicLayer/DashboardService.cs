using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SolarVolt.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context; // استبدل باسم الـ DbContext عندك

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetStatsAsync()
        {
            var stats = new DashboardStatsDto();

            // 1️⃣ الكروت العلوية
            stats.TotalSales = await _context.Orders
                .SumAsync(o => (decimal?)o.TotalCost) ?? 0;

            stats.TotalOrders = await _context.Orders.CountAsync();

            stats.LowStockProductsCount = await _context.Products
                .Where(p => p.StockQuantity < 5)
                .CountAsync();

            stats.AverageUserLoad = await _context.Energy_Input_Sessions
                .SelectMany(s => s.energy_Input_Items_List)
                .AverageAsync(i => (double?)i.WattOverride) ?? 0;

            // 2️⃣ مخطط المبيعات (آخر 7 أيام)
            stats.SalesOverview = await _context.Orders
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-7))
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesChartPointDto
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Amount = g.Sum(o => o.TotalCost)
                })
                .ToListAsync();

            // 3️⃣ المبيعات حسب الفئة
            stats.SalesByCategory = await (from item in _context.Order_Items
                                           join prod in _context.Products on item.ProductID equals prod.ProductId
                                           join cat in _context.Categories on prod.CategoryID equals cat.CategoryId
                                           group item by cat.Name into g
                                           select new CategorySalesDto
                                           {
                                               CategoryName = g.Key ?? "Uncategorized",
                                               Percentage = g.Sum(i => i.Quantity)
                                           }).ToListAsync();

            // 4️⃣ أحدث 5 طلبات
            stats.RecentOrders = await _context.Orders
                .OrderByDescending(o => o.OrderId)
                .Take(5)
                .Select(o => new RecentOrderDto
                {
                    OrderId = o.OrderId,
                    ClientName = o.user.FullName ?? "Client",
                    Date = o.OrderDate.ToString("yyyy-MM-dd"),
                    TotalAmount = o.TotalCost,
                    Status = o.Status.ToString()
                })
                .ToListAsync();

            return stats;
        }
    }
}