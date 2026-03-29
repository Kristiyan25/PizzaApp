namespace PizzaApp.Models
{
    public class CartItem
    {
        public int PizzaId { get; set; }
        public string PizzaName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}