using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaApp.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userName = User.Identity.Name;

        var query = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Pizza)
            .AsQueryable();

        
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(o => o.UserName == userName);
        }

        var orders = await query.ToListAsync();

        return View(orders);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        order.Status = status;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        _context.OrderItems.RemoveRange(order.Items); 
        _context.Orders.Remove(order);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}