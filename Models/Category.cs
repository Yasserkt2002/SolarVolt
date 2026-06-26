namespace SolarVolt.Models
{
    public class Category
    {
        public int CategoryId { get; set; } 
        public string Name { get; set; }   = string.Empty;   
        public bool IsDeleted { get; set; }=false;  



        public List<Product> products_List { get; set; } = new List<Product>();  
    }
}
