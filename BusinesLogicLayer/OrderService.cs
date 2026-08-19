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

        public async Task<ValidationResult> IsProductExists(CreateOrderDTo OrderDto, List<Product> SelectedProducts)   /////////// alot of logic //https://t.me/c/3394009212/2/131 شرح الدالة مع الاخطاء التي عملتا
        {
            // List<int> ProductIDs = OrderDto.OrderItems.Select(p => p.ProductID).ToList();

            var results = new ValidationResult();

            foreach (var p in OrderDto.OrderItems)
            {
                var product = SelectedProducts.FirstOrDefault(x => x.ProductId == p.ProductID); // تبحث عن المنتج المطابق

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

        public async Task<ValidationResult> IsProductQuantityAvailable(CreateOrderDTo OrderDto, List<Product> SelectedProducts)
        {
            //كود قديم قبل التحسينhttps://t.me/c/3394009212/2/132

            var results = new ValidationResult();

            foreach (var item in OrderDto.OrderItems)
            {
                var product = SelectedProducts.FirstOrDefault(p => p.ProductId == item.ProductID);

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
          //v1
        /*  public async Task<ValidationResult> CreateOrder(CreateOrderDTo OrderDto, int UserID)
          {
              List<int> ProductIDsList = GetProductIDsList(OrderDto);
              //تأخذ فقط IDs من الفرونت

              var Selec tedProducts = await _context.Products
                  .Where(p => ProductIDsList.Contains(p.ProductId) && !p.IsDeleted)
                  .ToListAsync(); //تجلب فقط المنتجات المطلوبة (بدون تحميل كل الداتابيز)

              ValidationResult validationProductExists = await IsProductExists(OrderDto, SelectedProducts);

              if (!validationProductExists.IsValid)
              {
                  return validationProductExists;
              }

              ValidationResult validationQuantityAvailable = await IsProductQuantityAvailable(OrderDto, SelectedProducts);

              if (!validationQuantityAvailable.IsValid)
              {
                  return validationQuantityAvailable;
              }

              var transaction=await _context.Database.BeginTransactionAsync();
              try
              {


                  Order order = new Order()
                  {
                      UserID = UserID,
                      OrderDate = DateTime.Now,
                      Status = "Pending",
                      TotalCost = 0, // أو القيمة المحسوبة

                      //  الحقول الجديدة بتنضاف هني
                      SubTotal = OrderDto.SubTotal, // أو حسب قيمتها عندك
                      DeliveryFee = 20, // أو OrderDto.DeliveryFee
                      Discount = OrderDto.Discount,
                      PaymentMethod = OrderDto.PaymentMethod,

                      // توليد كود الاستلام الـ 4 أرقام تلقائياً
                      DeliveryPin = new Random().Next(1000, 9999).ToString()
                  };
                  await _context.Orders.AddAsync(order); //الContext صار يراقب هذا الكائن     
                                                         // order.TotalCost += product.Cost * itemDto.Quantity; لاحقا عند هذا السطر بيعدلو من حالو



                  foreach (var itemDto in OrderDto.OrderItems)
                  {
                      var product = SelectedProducts.FirstOrDefault(p => p.ProductId == itemDto.ProductID);//يعني بتقارن كل عناصر اللست مع العنصر المقابل وبتكرر المقارنة كل دورة 
                      if (product != null)                                                                 //foreach
                      {
                          order.TotalCost += product.Cost * itemDto.Quantity;
                          product.StockQuantity -= itemDto.Quantity;
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
                  await transaction.CommitAsync();
              }
              catch (Exception)
              {
                  await transaction.RollbackAsync();
                  return new ValidationResult { IsValid = false }; //throw
              }
              return new ValidationResult { IsValid = true };
          }
        */

        //v2
        public async Task<ValidationResult> CreateOrder(CreateOrderDTo OrderDto, int UserID)
        {
            List<int> ProductIDsList = GetProductIDsList(OrderDto);

            var SelectedProducts = await _context.Products
                .Where(p => ProductIDsList.Contains(p.ProductId) && !p.IsDeleted)
                .ToListAsync();

            ValidationResult validationProductExists = await IsProductExists(OrderDto, SelectedProducts);
            if (!validationProductExists.IsValid)
            {
                return validationProductExists;
            }

            ValidationResult validationQuantityAvailable = await IsProductQuantityAvailable(OrderDto, SelectedProducts);
            if (!validationQuantityAvailable.IsValid)
            {
                return validationQuantityAvailable;
            }

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Order order = new Order()
                {
                    UserID = UserID,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    SubTotal = 0, // بنحسبه بالـ loop
                    DeliveryFee = OrderDto.DeliveryFee,
                    Discount = OrderDto.Discount,
                    PaymentMethod = OrderDto.PaymentMethod,
                    DeliveryPin = new Random().Next(1000, 9999).ToString()
                };

                await _context.Orders.AddAsync(order);

                foreach (var itemDto in OrderDto.OrderItems)
                {
                    var product = SelectedProducts.FirstOrDefault(p => p.ProductId == itemDto.ProductID);
                    if (product != null)
                    {
                        order.SubTotal += product.Cost * itemDto.Quantity; // تجميع السعر الفرعي
                        product.StockQuantity -= itemDto.Quantity;

                        Order_Item order_Item = new Order_Item()
                        {
                            ProductID = product.ProductId,
                            Quantity = itemDto.Quantity,
                            Price = product.Cost,
                            order = order
                        };
                        await _context.Order_Items.AddAsync(order_Item);
                    }
                }

                // حساب الإجمالي النهائي بأسلوب صحيح بعد تجميع الـ SubTotal
                order.TotalCost = (order.SubTotal + order.DeliveryFee) - order.Discount;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                return new ValidationResult
                {
                    IsValid = true,
                    OrderID = order.OrderId,
                    DeliverPin=order.DeliveryPin

                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return new ValidationResult { IsValid = false };
            }

          
        }
        public async Task<OrderResponseDTo> GetOrderByID(int OrderID)
        {
            
            var orderSelected =await _context.Orders.Include(i=>i.Order_Items_List).FirstOrDefaultAsync(o=>o.OrderId== OrderID&&o.UserID== 1   /*     userID     */);   ///////////////////////////////////////////////////////////////////////////////////////////////////userID
            if (orderSelected == null)
            {
                return null;
            }
            OrderResponseDTo orderResponseDTo = new OrderResponseDTo()
            {
                OrderId = orderSelected.OrderId,
                OrderDate = orderSelected.OrderDate,
                TotalCost = orderSelected.TotalCost,
                Status=orderSelected.Status,


                Order_Items_List = orderSelected.Order_Items_List.Select(o => new OrderItemResponseDTo()
                {
                    ProductID = o.ProductID,    
                    Quantity = o.Quantity,  
                    Price=o.Price,
                }).ToList()

            };
            return orderResponseDTo;
        }

        public async Task<List<GetAllUserOrdersDTo>> getAllUserOrders()
        {
            var AllOrders = await _context.Orders.Where(o=>o.UserID== 1    /*     userID     */).Select(o => new GetAllUserOrdersDTo()      ///////////////////////////////////////////////////////////////////////////////////////////////////userID
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalCost = o.TotalCost,
                Status=o.Status,
            }).ToListAsync();
            if (!AllOrders.Any())
            {
                return null;
            }
            return AllOrders;
        }

        public async Task<bool> CompletedOrder(int OrderID)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o=>o.OrderId== OrderID);
            if (order != null && order.Status == "Pending")
            {
                order.Status = "Completed";
                await _context.SaveChangesAsync();
                return true;
            }
            return false;


        }


        //verssion 1
        //public async Task<bool> CanceledOrder(int OrderID)
        //{

        //    var order =await _context.Orders.Include(i => i.Order_Items_List).ThenInclude(p => p.product).FirstOrDefaultAsync(o=>o.OrderId==OrderID);
        //    if (order != null && order.Status == "Pending")
        //    {
        //        foreach (var item in order.Order_Items_List)
        //        {
        //            if (item.product != null)
        //            {
        //                item.product.StockQuantity += item.Quantity;
        //            }
        //        }
        //        order.Status = "Canceled";
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    return false;
        //}

        //verssion 2
        public async Task<bool> CancelOrderAsync(int orderId, int currentUserId, string currentUserRole)
        {
            var order = await _context.Orders
                .Include(i => i.Order_Items_List)
                .ThenInclude(p => p.product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            // التحقق من وجود الطلب وحالته
            if (order == null || order.Status != "Pending")
            {
                return false;
            }

            // فحص الأمان: الكلاينت يلغي طلبه فقط، أما الأدمن فيحقل له إلغاء أي طلب
            if (currentUserRole == "Client" && order.UserID != currentUserId)
            {
                return false;
            }

            // إرجاع الكميات للمخزون
            foreach (var item in order.Order_Items_List)
            {
                if (item.product != null)
                {
                    item.product.StockQuantity += item.Quantity;
                }
            }

            order.Status = "Canceled";
            await _context.SaveChangesAsync();
            return true;
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

        public int OrderID { get; set; }
        public string DeliverPin { get; set; }
    }
}