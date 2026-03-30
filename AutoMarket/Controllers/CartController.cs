using Microsoft.AspNetCore.Mvc;
using PizzaApp.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class CartController : Controller
{
    private static List<CartItem> cart = new List<CartItem>();
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        return View(cart);
    }

    public IActionResult AddToCart(int id, string name, decimal price)
    {

        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            var item = cart.FirstOrDefault(p => p.PizzaId == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    PizzaId = id,
                    PizzaName = name,
                    Price = price,
                    Quantity = 1
                });
            }

            TempData["Success"] = "Pizza added to cart!";

            return RedirectToAction("Index", "Pizza");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        if (cart == null || !cart.Any())
            return RedirectToAction("Index");

        var order = new Order
        {
            UserName = User.Identity.Name ?? "Guest",
            CreatedAt = DateTime.Now,
            Status = "Pending",
            TotalPrice = cart.Sum(c => c.Price * c.Quantity),

            Items = cart.Select(c => new OrderItem
            {
                PizzaId = c.PizzaId,
                Quantity = c.Quantity,
                Price = c.Price
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        cart.Clear();

        return RedirectToAction("Index", "Orders");
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int id)
    {
        var item = cart.FirstOrDefault(p => p.PizzaId == id);

        if (item != null)
        {
            cart.Remove(item);
        }

        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Increase(int id)
    {
        var item = cart.FirstOrDefault(p => p.PizzaId == id);

        if (item != null)
        {
            item.Quantity++;
        }

        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Decrease(int id)
    {
        var item = cart.FirstOrDefault(p => p.PizzaId == id);

        if (item != null)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                TempData["Error"] = "Minimum quantity is 1";
            }
        }

        return RedirectToAction("Index");
    }
}