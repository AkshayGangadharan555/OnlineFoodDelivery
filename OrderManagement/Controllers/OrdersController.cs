
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Models;

public class OrdersController : Controller
{
    private readonly OrdersContext _context;

    public OrdersController(OrdersContext context)
    {
        _context = context;
    }

    // GET: ORDERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Orders.ToListAsync());
    }

    // GET: ORDERS/Details/5
    public async Task<IActionResult> Details(System.Guid? orderid)
    {
        if (orderid == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(m => m.OrderId == orderid);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: ORDERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ORDERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("OrderId,CustomerId,RestaurantId,DeliveryManId,OrderDate,Status,TotalAmount,PaymentAddressId,DeliveryAddressId,ExpectedDeliveryTime,ActualDeliveryTime,CreatedAt,UpdatedAt,RowVersion,Items")] Order order)
    {
        if (ModelState.IsValid)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(order);
    }

    // GET: ORDERS/Edit/5
    public async Task<IActionResult> Edit(System.Guid? orderid)
    {
        if (orderid == null)
        {
            return NotFound();
        }

        var order = await _context.Orders.FindAsync(orderid);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    // POST: ORDERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(System.Guid? orderid, [Bind("OrderId,CustomerId,RestaurantId,DeliveryManId,OrderDate,Status,TotalAmount,PaymentAddressId,DeliveryAddressId,ExpectedDeliveryTime,ActualDeliveryTime,CreatedAt,UpdatedAt,RowVersion,Items")] Order order)
    {
        if (orderid != order.OrderId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.OrderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(order);
    }

    // GET: ORDERS/Delete/5
    public async Task<IActionResult> Delete(System.Guid? orderid)
    {
        if (orderid == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(m => m.OrderId == orderid);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: ORDERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(System.Guid? orderid)
    {
        var order = await _context.Orders.FindAsync(orderid);
        if (order != null)
        {
            _context.Orders.Remove(order);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(System.Guid? orderid)
    {
        return _context.Orders.Any(e => e.OrderId == orderid);
    }
}
