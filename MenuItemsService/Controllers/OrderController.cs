using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenuItemsService.Models;

namespace MenuItemsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ItemContext _context;

        public OrdersController(ItemContext context)
        {
            _context = context;
        }


        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetOrdersByRestaurant(string restaurantId)
        {
            // o.RestaurantId columns check loop maps
            var orders = await _context.Orders
                .Where(o => o.RestaurantId == restaurantId)
                .ToListAsync();
            return Ok(orders);
        }


        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromQuery] string status)
        {
            // Orders dynamic container target item find location
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound("Order not found");

            order.OrderStatus = status; // Placed -> Accepted -> Preparing -> Delivered
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Order status successfully changed to {status}" });
        }


        [HttpGet("restaurant/{restaurantId}/report")]
        public async Task<IActionResult> GetOrderReport(string restaurantId, [FromQuery] string period)
        {
            var query = _context.Orders.Where(o => o.RestaurantId == restaurantId);
            var today = DateTime.Today;

            if (period.ToLower() == "day")
            {
                query = query.Where(o => o.OrderDate.Date == today);
            }
            else if (period.ToLower() == "week")
            {
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                query = query.Where(o => o.OrderDate.Date >= startOfWeek);
            }
            else if (period.ToLower() == "month")
            {
                query = query.Where(o => o.OrderDate.Month == today.Month && o.OrderDate.Year == today.Year);
            }

            var result = await query.ToListAsync();
            return Ok(result);
        }

        //  Fixed Line: [FromBody] tarvatha 'Orders' ani marchanu!
        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] Orders newOrder)
        {
            if (newOrder == null)
            {
                return BadRequest("Order data cannot be null");
            }

            var totalOrders = await _context.Orders.CountAsync();

            int nextIdNumber = totalOrders + 1;
            newOrder.OrderId = "ORD-" + nextIdNumber.ToString("D3");

            newOrder.OrderDate = DateTime.Now;
            newOrder.OrderStatus = "Placed";

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return Ok(newOrder);
        }
    }
}