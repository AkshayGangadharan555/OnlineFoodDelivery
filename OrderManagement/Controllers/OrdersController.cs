
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Models;
using OrderEntity = Orders.Models.Orders;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    public readonly OrdersContext _context;

    public OrdersController(OrdersContext context)
    {
        _context = context;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderEntity>>> Get(CancellationToken cancellationToken)
    {
        var list = await _context.Orders.AsNoTracking().ToListAsync(cancellationToken);
        return Ok(list);
    }

    // GET: api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderEntity>> Get(Guid id, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(m => m.OrderId == id, cancellationToken);
        if (order == null)
            return NotFound();
        return order;
    }

    // GET: api/orders/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<OrderEntity>>> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders.AsNoTracking().Where(o => o.CustomerId == userId).ToListAsync(cancellationToken);
        return Ok(orders);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<OrderEntity>> Create(OrderEntity orders, CancellationToken cancellationToken)
    {
        if (orders == null)
            return BadRequest();

        if (orders.OrderId == Guid.Empty)
            orders.OrderId = Guid.NewGuid();

        _context.Orders.Add(orders);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = orders.OrderId }, orders);
    }

    // PUT: api/orders/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, OrderEntity orders, CancellationToken cancellationToken)
    {
        if (id != orders.OrderId)
            return BadRequest();

        _context.Entry(orders).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrdersExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    // PUT: api/orders/{id}/status
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(new object[] { id }, cancellationToken);
        if (order == null)
            return NotFound();

        order.Status = status;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    // DELETE: api/orders/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FindAsync(new object[] { id }, cancellationToken);
        if (order == null)
            return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private bool OrdersExists(Guid id)
    {
        return _context.Orders.Any(e => e.OrderId == id);
    }
}
