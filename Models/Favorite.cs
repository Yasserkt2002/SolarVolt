namespace SolarVolt.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationships
        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;

        // ملاحظة: تأكد من اسم خاصية المفتاح الرئيسي بـ Product (ProductID أو Id) وتطبيقه بالـ DbContext
    }
}