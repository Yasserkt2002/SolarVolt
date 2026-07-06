using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using SolarVolt.DTOs;
using SolarVolt.Models;
using System.Linq;
using static BusinesLogicLayer.ValidationResult;

namespace BusinesLogicLayer
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

            public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ValidationResult> IsProductExists(CreateOrderDTo OrderDto, List<Product> productsListIDSelectedDB)   /////////// alot of logic //https://t.me/c/3394009212/2/131 شرح الدالة مع الاخطاء التي عملتا
        {
            // List<int> ProductIDs = OrderDto.OrderItems.Select(p => p.ProductID).ToList();

            var results = new ValidationResult();

            foreach (var p in OrderDto.OrderItems)
            {
                var product = productsListIDSelectedDB.FirstOrDefault(x => x.ProductId == p.ProductID); // تبحث عن المنتج المطابق

                if (product == null)
                {
                    results.InvalidProductIDs.Add(p.ProductID);
                }
            }

            if (results.InvalidProductIDs.Count > 0)
                results.enProductOrQuanitiy = ProductOrQuanitiy.ProductNotExists;

            results.IsValid = results.InvalidProductIDs.Count == 0;

            return results;
        }

        public async Task<ValidationResult> IsProductQuantityAvailable(CreateOrderDTo OrderDto, List<Product> productsListIDSelectedDB)
        {
            //كود قديم قبل التحسينhttps://t.me/c/3394009212/2/132

            var results = new ValidationResult();

            foreach (var item in OrderDto.OrderItems)
            {
                var product = productsListIDSelectedDB.FirstOrDefault(p => p.ProductId == item.ProductID);

                // حماية من
                // null 
                if (product == null)
                {
                    results.InvalidProductIDs.Add(item.ProductID);
                    continue;
                }

                if (item.Quantity > product.StockQuantity)
                {
                    results.InvalidProductIDs.Add(item.ProductID);
                }
            }

            if (results.InvalidProductIDs.Count > 0)
                results.enProductOrQuanitiy = ProductOrQuanitiy.QuantityLessThenOrder;

            results.IsValid = results.InvalidProductIDs.Count == 0;

            return results;   //return NotFound(new {message=this products han no Enoghf quanitiy,data=Vaidation.InvalidProductIDs})
        }

        List<int> GetProductIDsList(CreateOrderDTo OrderDto)
        {
            return OrderDto.OrderItems.Select(o => o.ProductID).ToList();
        } //تأخذ فقط IDs من الفرونت

        public async Task<ValidationResult> CreateOrder(CreateOrderDTo OrderDto, int UserID)
        {
            List<int> ProductIDsList = GetProductIDsList(OrderDto);

            //تأخذ فقط IDs من الفرونت

            var productsIDsListSelectedDB = await _context.Products
                .Where(p => ProductIDsList.Contains(p.ProductId) && !p.IsDeleted)
                .ToListAsync(); //تجلب فقط المنتجات المطلوبة (بدون تحميل كل الداتابيز)

            ValidationResult validationProductExists = await IsProductExists(OrderDto, productsIDsListSelectedDB);

            if (!validationProductExists.IsValid)
            {
                return validationProductExists;
            }

            ValidationResult validationQuantityAvailable = await IsProductQuantityAvailable(OrderDto, productsIDsListSelectedDB);

            if (!validationQuantityAvailable.IsValid)
            {
                return validationQuantityAvailable;
            }


            Order order = new Order()
            {
                UserID = UserID,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalCost = 0,
               
            };
            Console.WriteLine("i am before foreach and after Order =new Order");

            await _context.Orders.AddAsync(order);


            // اطبعهون قبل حلقة الـ foreach مباشرة
            Console.WriteLine($"👉 DB Products Count: {productsIDsListSelectedDB.Count}");
            Console.WriteLine($"👉 DTO Items Count: {OrderDto.OrderItems.Count}");

            foreach (var itemDto in OrderDto.OrderItems)
            {
                var product = productsIDsListSelectedDB.FirstOrDefault(p => p.ProductId == itemDto.ProductID);
                if (product != null)
                {
                    order.TotalCost += product.Cost * itemDto.Quantity;
                    product.StockQuantity-=itemDto.Quantity;
                    Order_Item order_Item = new Order_Item()
                    {
                        ProductID = product.ProductId,
                        Quantity = itemDto.Quantity,
                        Price = product.Cost, // price at sell
                        order = order
                    };
                    await _context.Order_Items.AddAsync(order_Item); 
                }
            }
            await _context.SaveChangesAsync();  

            return new ValidationResult { IsValid = true };
        }

    }

    public class ValidationResult
    {
        public enum ProductOrQuanitiy
        {
            ProductNotExists,
            QuantityLessThenOrder
        }

        public bool IsValid { get; set; }

        public ProductOrQuanitiy enProductOrQuanitiy;

        public int ProductID { get; set; }

        public List<int> InvalidProductIDs { get; set; } = new List<int>();
    }
}