using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.DTOs.Request;
using Orders.Services.Interfaces;
using System.Security.Claims;

namespace Orders.Controllers
{
    [ApiController]
    [Route("api/orders/restaurant")]
    [Authorize(Roles = "Restaurant")]
    public class RestaurantOrdersController : ControllerBase
    {
        private readonly IRestaurantOrderService _service;

        public RestaurantOrdersController(IRestaurantOrderService service)
        {
            _service = service;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetRestaurantOrders()
        {
            var restaurantId =Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result =await _service.GetRestaurantOrdersAsync(restaurantId);

            return Ok(result);
        }

        [HttpPut("accept/{orderId}")]
        public async Task<IActionResult>AcceptOrder(Guid orderId,[FromBody]byte[] rowVersion)
        {
            var restaurantId =Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result =await _service.AcceptOrderAsync(orderId,restaurantId,rowVersion);

            if (!result)
                return BadRequest("Order accept failed");

            return Ok("Order accepted");
        }

        [HttpPatch("update-status/{orderId}")]
        public async Task<IActionResult>UpdateStatus(Guid orderId, UpdateOrderStatusRequestDto request)
        {
            var userId =User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token");
            }
            var restaurantId =Guid.Parse(userId);
            var result =await _service.UpdateOrderStatusAsync(request,restaurantId);

            if (!result)
                return BadRequest("Status update failed");

            return Ok("Order updated");
        }

    }
}