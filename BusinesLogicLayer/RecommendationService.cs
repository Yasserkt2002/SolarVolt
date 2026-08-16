using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;

namespace SolarVolt.BusinesLogicLayer
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
            int totalWatt = session.TotalWatt; // أقصى حمل لحظي
            double totalConsumingWh = session.energy_Input_Items_List
                .Sum(item => item.Quantity * item.OperatingHours * (item.WattOverride ?? 0));

            // 3. حساب الاحتياجات الفيزيائية المطلوبة
            double requiredInverterWatt = totalWatt * 1.25; // مع هامش أمان 25%
            double requiredSolarWatt = (totalConsumingWh * 1.2) / 4.5; // بافتراض 4.5 ساعات شمس

            // 4. جلب المنتجات المناسبة من جدول Products
            // أ) الإنفرتر المناسب
            var selectedInverter = await _context.Products
                .Where(p => p.CategoryID == INVERTER_CATEGORY_ID && !p.IsDeleted && p.WattCapacity >= requiredInverterWatt)
                .OrderBy(p => p.WattCapacity)
                .FirstOrDefaultAsync()
                ?? await _context.Products
                    .Where(p => p.CategoryID == INVERTER_CATEGORY_ID && !p.IsDeleted)
                    .OrderByDescending(p => p.WattCapacity)
                    .FirstOrDefaultAsync();

            // ب) اللوح الشمسي المناسب
            var selectedPanel = await _context.Products
                .Where(p => p.CategoryID == SOLAR_PANEL_CATEGORY_ID && !p.IsDeleted)
                .OrderByDescending(p => p.WattCapacity)
                .FirstOrDefaultAsync();

            // ج) البطارية المناسبة
            var selectedBattery = await _context.Products
                .Where(p => p.CategoryID == BATTERY_CATEGORY_ID && !p.IsDeleted )
                .OrderByDescending(p => p.WattCapacity)
                .FirstOrDefaultAsync();

            // 5. حساب الكميات والأسعار
            int panelCount = (selectedPanel != null && selectedPanel.WattCapacity > 0)
                ? (int)Math.Ceiling(requiredSolarWatt / selectedPanel.WattCapacity)
                : 0;

            int batteryCount = (selectedBattery != null && selectedBattery.WattCapacity > 0)
                ? (int)Math.Ceiling((totalConsumingWh * 0.6) / selectedBattery.WattCapacity) // تغطية الاستهلاك الليلي
                : 0;

            decimal inverterCost = selectedInverter?.Cost ?? 0m;
            decimal panelsTotalCost = (selectedPanel?.Cost ?? 0m) * panelCount;
            decimal batteriesTotalCost = (selectedBattery?.Cost ?? 0m) * batteryCount;
            decimal totalEstimatedCost = inverterCost + panelsTotalCost + batteriesTotalCost;

            // 6. تجهيز الـ DTO النهائي للعرض بصفحة الفلاتر
            return new RecommendationDTO
            {
                SessionID = SessionID,
                TotalEnergyWh = totalConsumingWh,

                // الإنفرتر
                RecommendationInverterKw = selectedInverter != null ? Math.Round(selectedInverter.WattCapacity / 1000.0, 1) : 0,

                // البطاريات
                NumberOfBatteries = batteryCount,
                RecommendationBettaryAh = selectedBattery != null ? selectedBattery.WattCapacity : 0,
                BatteryType = selectedBattery?.Name ?? "N/A",

                // الألواح
                RecommendationPanelCount = panelCount,
                RecommendationPanelWattage = selectedPanel != null ? selectedPanel.WattCapacity : 0,

                // التكلفة والإنتاج
                EstimatedCost = totalEstimatedCost,
                MonthlyProductionKWh = Math.Round((requiredSolarWatt * 4.5 * 30) / 1000.0, 1)
            }; 
        }
    }
}