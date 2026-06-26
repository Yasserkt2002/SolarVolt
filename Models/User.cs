namespace SolarVolt.Models
{
    public class User
    {
        public int UserID { set; get; } 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;



        public List<Order> Orders_List { get; set; } = new List<Order>();

        public List<Recommendation> Recommendations_List { get; set; } = new List<Recommendation>();

        public List<Energy_Input_Session> Energy_Input_Sessions_List=new List<Energy_Input_Session>();


    }
}
