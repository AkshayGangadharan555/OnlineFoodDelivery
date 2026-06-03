using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.DTOs.Request;
using Orders.Services.Interfaces;
using System.Security.Claims;

namespace Orders.Controllers
{
    [ApiController]
    [Route("api/orders/delivery-agent")]
    [Authorize(Roles ="DeliveryAgent")]
    public class DeliveryOrdersController : ControllerBase
    {
        private readonly IDeliveryOrderService _service;

        public DeliveryOrdersController(IDeliveryOrderService service)
        {
            _service = service;
        }

        [HttpGet("available-orders")]
        public async Task<IActionResult>GetAvailableOrders()
        {
            var result =await _service.GetAvailableOrdersAsync();

            return Ok(result);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult>GetAssignedOrders()
        {
            var deliveryAgentId =Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result =await _service.GetAssignedOrdersAsync(deliveryAgentId);

            return Ok(result);
        }

        [HttpPatch("accept")]
        public async Task<IActionResult> AcceptDelivery(AssignDeliveryRequestDto request)
        {
            var result =await _service.AcceptDeliveryAsync(request);

            if (!result)
                return BadRequest("Delivery accept failed");

            return Ok(
                "Delivery accepted");
        }

        [HttpPatch("pickup")]
        public async Task<IActionResult>PickupOrder(UpdateOrderStatusRequestDto request)
        {
            var deliveryAgentId =Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result =await _service.PickupOrderAsync(request,deliveryAgentId);

            if (!result)
                return BadRequest("Pickup failed");

            return Ok("Order picked");
        }

        [HttpPatch("deliver")]
        public async Task<IActionResult>DeliverOrder(UpdateOrderStatusRequestDto request)
        {
            var deliveryAgentId =Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result =await _service.DeliverOrderAsync(request,deliveryAgentId);

            if (!result)
                return BadRequest("Delivery failed");

            return Ok(
                "Order delivered");
        }
    }
}