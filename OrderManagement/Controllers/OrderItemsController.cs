
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Models;

public class OrderItemsController : Controller
{
    private readonly OrdersContext _context;

    public OrderItemsController(OrdersContext context)
    {
        _context = context;
    }

    // GET: ORDERITEMSS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.OrderItems.ToListAsync());
    }

    // GET: ORDERITEMSS/Details/5
    public async Task<IActionResult> Details(System.Guid? orderitemid)
    {
        if (orderitemid == null)
        {
            return NotFound();
        }

        var orderitems = await _context.OrderItems
            .FirstOrDefaultAsync(m => m.OrderItemId == orderitemid);
        if (orderitems == null)
        {
            return NotFound();
        }

        return View(orderitems);
    }

    // GET: ORDERITEMSS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ORDERITEMSS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("OrderItemId,OrderId,ProductId,RestuarantId,Quantity,UnitPrice,Discount,TaxAmount,ItemDescription,SubTotal,SpecialInstructions,Status,CreatedAt,LastUpdatedAt")] OrderItems orderitems)
    {
        if (ModelState.IsValid)
        {
            _context.Add(orderitems);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(orderitems);
    }

    // GET: ORDERITEMSS/Edit/5
    public async Task<IActionResult> Edit(System.Guid? orderitemid)
    {
        if (orderitemid == null)
        {
            return NotFound();
        }

        var orderitems = await _context.OrderItems.FindAsync(orderitemid);
        if (orderitems == null)
        {
            return NotFound();
        }
        return View(orderitems);
    }

    // POST: ORDERITEMSS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(System.Guid? orderitemid, [Bind("OrderItemId,OrderId,ProductId,RestuarantId,Quantity,UnitPrice,Discount,TaxAmount,ItemDescription,SubTotal,SpecialInstructions,Status,CreatedAt,LastUpdatedAt")] OrderItems orderitems)
    {
        if (orderitemid != orderitems.OrderItemId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(orderitems);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemsExists(orderitems.OrderItemId))
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
        return View(orderitems);
    }

    // GET: ORDERITEMSS/Delete/5
    public async Task<IActionResult> Delete(System.Guid? orderitemid)
    {
        if (orderitemid == null)
        {
            return NotFound();
        }

        var orderitems = await _context.OrderItems
            .FirstOrDefaultAsync(m => m.OrderItemId == orderitemid);
        if (orderitems == null)
        {
            return NotFound();
        }

        return View(orderitems);
    }

    // POST: ORDERITEMSS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(System.Guid? orderitemid)
    {
        var orderitems = await _context.OrderItems.FindAsync(orderitemid);
        if (orderitems != null)
        {
            _context.OrderItems.Remove(orderitems);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderItemsExists(System.Guid? orderitemid)
    {
        return _context.OrderItems.Any(e => e.OrderItemId == orderitemid);
    }
}
