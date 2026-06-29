namespace SolarVolt.Models
{
    public class Product
    {
        public int ProductId { get; set; } 
        public string Name { get; set; } = string.Empty;


        public string Brand { get; set; } = string.Empty;   

        public int StockQuantity { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public int WattCapacity { get; set; }

        public string ImagePath { get; set; }= string.Empty;
        public int CategoryID { get; set; }
        public bool IsDeleted { get; set; }


        public Category Category { get; set; } = null!;
        //// https://t.me/c/3394009212/2/70
       


        public List<Order_Item> Order_Items_List { get; set; } = new List<Order_Item>();
       // https://t.me/c/3394009212/2/71
        

        public List<Product_Unit> Product_Units_List=new List<Product_Unit>();

        public List<Recommendation_Item> Recommendation_Items_List=new List<Recommendation_Item>(); 

    }
}
