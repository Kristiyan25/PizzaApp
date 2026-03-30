using System.ComponentModel.DataAnnotations;

namespace PizzaApp.Models
{
    public class Order
    {
        public int Id { get; set; }

       
        public string UserName { get; set; } = "";

        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        
        public decimal TotalPrice { get; set; }

        
        public string Status { get; set; } = "Pending";

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}