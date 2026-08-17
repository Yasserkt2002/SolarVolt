using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;
using SolarVolt.Models;

namespace BusinesLogicLayer
{
    public class RecommendationService
    {
        private readonly ApplicationDbContext _context;

        // معرفات التصنيفات حسب جدول Categories عندك
        private const int BATTERY_CATEGORY_ID = 1;
        private const int INVERTER_CATEGORY_ID = 2;
        private const int SOLAR_PANEL_CATEGORY_ID = 5;

        public RecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RecommendationDTO?> CalculateRecommendation(int SessionID, int UserID)
        {
            // 1. جلب الجلسة مع عناصرها
            var session = await _context.Energy_Input_Sessions
                .Include(e => e.energy_Input_Items_List)
                .FirstOrDefaultAsync(e => e.Energy_Input_SessionID == SessionID && e.UserID == UserID);

            if (session == null || !session.energy_Input_Items_List.Any())
                return null;

            // 2. حساب أقصى قدرة لحظية وإجمالي الاستهلاك اليومي (Wh)
            int totalWatt = session.TotalWatt;
            double totalConsumingWh = session.energy_Input_Items_List
                .Sum(item => item.Quantity * item.OperatingHours * (item.WattOverride ?? 0));

            // 3. حساب الاحتياجات الفيزيائية المطلوبة
            double requiredInverterWatt = totalWatt * 1.25;
            double requiredSolarWatt = (totalConsumingWh * 1.2) / 4.5;

            // 4. جلب المنتجات المناسبة من جدول Products (بغض النظر عن المخزون)
            var selectedInverter = await _context.Products
                .Where(p => p.CategoryID == INVERTER_CATEGORY_ID && !p.IsDeleted && p.WattCapacity >= requiredInverterWatt)
                .OrderBy(p => p.WattCapacity)
                .FirstOrDefaultAsync()
                ?? await _context.Products
                    .Where(p => p.CategoryID == INVERTER_CATEGORY_ID && !p.IsDeleted)
                    .OrderByDescending(p => p.WattCapacity)
                    .FirstOrDefaultAsync();

            var selectedPanel = await _context.Products
                .Where(p => p.CategoryID == SOLAR_PANEL_CATEGORY_ID && !p.IsDeleted)
                .OrderByDescending(p => p.WattCapacity)
                .FirstOrDefaultAsync();

            var selectedBattery = await _context.Products
                .Where(p => p.CategoryID == BATTERY_CATEGORY_ID && !p.IsDeleted)
                .OrderByDescending(p => p.WattCapacity)
                .FirstOrDefaultAsync();

            // 5. معالجة حالة عدم توفر الأصناف بكتالوج النظام
            if (selectedInverter == null || selectedPanel == null||  selectedBattery == null)
            {
                throw new InvalidOperationException("تعذر إعداد التوصية: لا توجد أصناف منتجات مطابقة بالمواصفات بكتالوج النظام.");
            }

            // 6. حساب الكميات والأسعار
            int panelCount = selectedPanel.WattCapacity > 0
                ? (int)Math.Ceiling(requiredSolarWatt / selectedPanel.WattCapacity)
                : 0;

            int batteryCount = selectedBattery.WattCapacity > 0
                ? (int)Math.Ceiling((totalConsumingWh * 0.6) / selectedBattery.WattCapacity)
                : 0;

            decimal inverterCost = selectedInverter.Cost;
            decimal panelsTotalCost = selectedPanel.Cost * panelCount;
            decimal batteriesTotalCost = selectedBattery.Cost * batteryCount;
            decimal totalEstimatedCost = inverterCost + panelsTotalCost + batteriesTotalCost;

            // 7. حفظ التوصية وعناصرها بالداتابيز
            var recommendation = new Recommendation
            {
                UserID = UserID,
                SessionID = SessionID,
                TotalWattage = totalWatt,
                EstimatedCost = totalEstimatedCost,
                CreatedAt = DateTime.UtcNow,
                Recommendation_Items_List = new List<Recommendation_Item>
                {
                    new Recommendation_Item { ProductID = selectedInverter.ProductId, Quantity = 1 },
                    new Recommendation_Item { ProductID = selectedPanel.ProductId, Quantity = panelCount },
                    new Recommendation_Item { ProductID = selectedBattery.ProductId, Quantity = batteryCount }
                }
            };

            _context.Recommendations.Add(recommendation);
            await _context.SaveChangesAsync();

            // 8. تجهيز الـ DTO النهائي للعرض
            // 8. تجهيز الـ DTO النهائي للعرض
            return new RecommendationDTO
            {
                SessionID = SessionID,
                TotalEnergyWh = totalConsumingWh,

                // الإنفرتر
                RecommendationInverterKw = Math.Round(selectedInverter.WattCapacity / 1000.0, 1),

                // البطاريات
                NumberOfBatteries = batteryCount,

                // 👇 التعديل هنا: تقسيم الـ WattCapacity على الفولتية (48V) للحصول على Ah الحقيقي
                RecommendationBatteryAh = (int)Math.Round((selectedBattery.WattCapacity * batteryCount) / 48.0),

                BatteryType = selectedBattery.Name,

                // الألواح
                RecommendationPanelCount = panelCount,
                RecommendationPanelWattage = selectedPanel.WattCapacity,

                // التكلفة والإنتاج
                EstimatedCost = totalEstimatedCost,
                MonthlyProductionKWh = Math.Round((requiredSolarWatt * 4.5 * 30) / 1000.0, 1)
                 
            };
        }
    }
}