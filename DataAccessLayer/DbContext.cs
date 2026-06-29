
    using Microsoft.EntityFrameworkCore;
    using SolarVolt.Models;

    namespace DataAccessLayer
    {
        public class ApplicationDbContext : DbContext
        {
            // الـ Constructor الأساسي لتمرير إعدادات الاتصال لقاعدة البيانات
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {

            }
                // جداول المستخدمين والطلبات
                public DbSet<User> Users { get; set; }
                public DbSet<Order> Orders { get; set; }
                public DbSet<Order_Item> Order_Items { get; set; }

                // جداول المنتجات والأصناف
                public DbSet<Product> Products { get; set; }
                public DbSet<Category> Categories { get; set; }
                public DbSet<Product_Unit> Product_Units { get; set; }
                public DbSet<Appliance> Appliances { get; set; }

                // جداول حاسبة الأحمال والتوصيات
                public DbSet<Energy_Input_Session> Energy_Input_Sessions { get; set; }
                public DbSet<Energy_Input_Item> Energy_Input_Items { get; set; }
                public DbSet<Recommendation> Recommendations { get; set; }
                public DbSet<Recommendation_Item> Recommendation_Items { get; set; }



        //  دالة تجديد حجم العواميد   nvarchar.....
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. تظبيط حقول الـ Decimal (الدقة والـ Scale)
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order_Item>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.Cost) // تأكد من حالة الأحرف Cost أو cost في الكلاس
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Recommendation>()
                .Property(r => r.EstimatedCost)
                .HasColumnType("decimal(18,2)");

            // 2. تحديد أحجام الـ Strings
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(50);
                entity.Property(u => u.LastName).HasMaxLength(50);
                entity.Property(u => u.Email).HasMaxLength(150);
                entity.Property(u => u.Phone).HasMaxLength(20);
                entity.Property(u => u.Role).HasMaxLength(30);
            });

            modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(150);
            modelBuilder.Entity<Category>().Property(c => c.Name).HasMaxLength(100);

            // ==========================================
            // 3. 

            // [1] علاقة الـ One-to-One بين الجلسة والتوصية
            modelBuilder.Entity<Recommendation>()
                .HasOne(r => r.energy_Input_Session)
                .WithOne(s => s.recommendation)
                .HasForeignKey<Recommendation>(r => r.SessionID) // تم تعديل الاسم ليطابق الـ ERD (SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // [2] علاقة المستخدم مع الطلبات (Users -> Orders)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.user)
                .WithMany(u => u.Orders_List)
                .HasForeignKey(o => o.UserID);

            // [3] علاقة الصنف مع المنتجات (Categories -> Products)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.products_List)
                .HasForeignKey(p => p.CategoryID);

            // [4] علاقة الطلب مع عناصر الطلب (Orders -> Order_Items)
            modelBuilder.Entity<Order_Item>()
                .HasOne(oi => oi.order)
                .WithMany(o => o.Order_Items_List)
                .HasForeignKey(oi => oi.OrderID);

            // [5] علاقة الجلسة مع عناصر الجلسة (Energy_Input_Sessions -> Energy_Input_Items)
            modelBuilder.Entity<Energy_Input_Item>()
                .HasOne(eii => eii.energy_Input_Session)
                .WithMany(eis => eis.energy_Input_Items_List)
                .HasForeignKey(eii => eii.SessionID); 


            // [6] علاقة المنتجات مع الوحدات (Products -> Product_Units)
            modelBuilder.Entity<Product_Unit>()
                .HasOne(pu => pu.product)
                .WithMany(p => p.Product_Units_List)
                .HasForeignKey(pu => pu.ProductID);

            // [7] علاقة الأجهزة مع عناصر الجلسة (Appliances -> Energy_Input_Items)
            modelBuilder.Entity<Energy_Input_Item>()
                .HasOne(eii => eii.appliance)
                .WithMany(a => a.Energy_Input_Items_List)
                .HasForeignKey(eii => eii.ApplianceID);

            // [8] علاقة المستخدم مع جلسات المدخلات (Users -> Energy_Input_Sessions)
            modelBuilder.Entity<Energy_Input_Session>()
                .HasOne(eis => eis.user)
                .WithMany(u => u.Energy_Input_Sessions_List)
                .HasForeignKey(eis => eis.UserID);

            // [9] علاقة المستخدم مع التوصيات (Users -> Recommendations)
            modelBuilder.Entity<Recommendation>()
                .HasOne(r => r.user)
                .WithMany(u => u.Recommendations_List)
                .HasForeignKey(r => r.UserID);

            // [10] علاقة التوصية مع عناصر التوصية (Recommendations -> Recommendation_Items)
            modelBuilder.Entity<Recommendation_Item>()
                .HasOne(ri => ri.recommendation)
                .WithMany(r => r.Recommendation_Items_List)
                .HasForeignKey(ri => ri.RecommendationID);

            // [11] علاقة المنتج مع عناصر التوصية (Products -> Recommendation_Items)
            modelBuilder.Entity<Recommendation_Item>()
                .HasOne(ri => ri.product)
                .WithMany(p => p.Recommendation_Items_List)
                .HasForeignKey(ri => ri.ProductID);

            // [12] علاقة المنتج مع عناصر الطلب (Products -> Order_Items)
            modelBuilder.Entity<Order_Item>()
                .HasOne(oi => oi.product)
                .WithMany(p => p.Order_Items_List)
                .HasForeignKey(oi => oi.ProductID);
        }
        //// شرح ما سبق https://t.me/c/4223720460/12/27
    }
}

