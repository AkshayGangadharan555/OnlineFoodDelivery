using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.DTOs.Request;
using Orders.Services.Interfaces;
using System.Security.Claims;

namespace Orders.Controllers
{
    [ApiController]
    [Route("api/orders/customer")]
    [Authorize(Roles = "Customer")]
    public class CustomerOrdersController : ControllerBase
    {
        private readonly ICustomerOrderService _service;

        public CustomerOrdersController( ICustomerOrderService service)
        { 
            _service = service;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult>PlaceOrder(PlaceOrderRequestDto request)
        {
            var token =HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ","");
            var result =await _service.PlaceOrderAsync(request,token);

            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult>GetOrderById(Guid orderId)
        {
            var customerId =Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result =await _service.GetOrderByIdAsync(orderId,customerId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult>GetMyOrders()
        {
            var customerId =Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result =await _service.GetCustomerOrdersAsync(customerId);

            return Ok(result);
        }

        [HttpPatch("cancel/{orderId}")]
        public async Task<IActionResult>CancelOrder(Guid orderId, CancelOrderRequestDto request)
        {
            var userId =User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token");
            }
            var customerId =Guid.Parse(userId);
            var result =await _service.CancelOrderAsync(request,customerId);

            if (!result)
                return BadRequest("Unable to cancel order");

            return Ok("Order cancelled");
        }
    }
}