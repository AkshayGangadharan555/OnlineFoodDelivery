using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.DTOs.RequestDTOs;
using Orders.DTOs.ResponseDTOs;
using Orders.Models;
using Orders.Services.Interfaces;

namespace Orders.Controllers
{
    /// <summary>
    /// Controller for managing orders.
    /// Provides operations to place, retrieve, and manage orders.
    /// Requires JWT Bearer token authentication for all operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;

        /// <summary>
        /// Initializes a new instance of the OrdersApiController.
        /// </summary>
        /// <param name="orderService">Service for order operations</param>
        public OrdersApiController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Place a new order
        /// </summary>
        /// <param name="request">Request containing order details and items</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Created Order entity</returns>
        /// <response code="201">Order placed successfully</response>
        /// <response code="400">Invalid request data</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Order>>> PlaceOrder(
            [FromBody] CreateOrderRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<Order>(400, "Invalid request data", null));

            var customerId = User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(customerId, out var customerGuid))
                return Unauthorized(new ApiResponse<Order>(401, "Invalid or missing customer ID", null));

            var result = await _orderService.PlaceOrderAsync(request, customerGuid, cancellationToken);

            if (result.Status == 201)
                return CreatedAtAction(nameof(GetOrder), new { orderId = result.Data?.OrderId }, result);

            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>OrderResponse containing the order details</returns>
        /// <response code="200">Returns the requested order</response>
        /// <response code="404">Order not found</response>
        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrder(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrderAsync(orderId, cancellationToken);

            if (result.Status == 404)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="orderId">The ID of the order to update</param>
        /// <param name="request">Request containing the new status</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Updated Order entity</returns>
        /// <response code="200">Order status updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Order not found</response>
        [HttpPut("{orderId}/status")]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Order>>> UpdateStatus(
            Guid orderId,
            [FromBody] UpdateOrderStatusRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<Order>(400, "Invalid request data", null));

            var result = await _orderService.UpdateStatusAsync(orderId, request.Status, cancellationToken);

            if (result.Status == 404)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Accept an order (delivery agent action)
        /// </summary>
        /// <param name="orderId">The ID of the order to accept</param>
        /// <param name="request">Request containing delivery agent ID</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Updated Order entity with delivery agent assigned</returns>
        /// <response code="200">Order accepted successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Order not found</response>
        [HttpPost("{orderId}/accept")]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Order>>> AcceptOrder(
            Guid orderId,
            [FromBody] AcceptRejectOrderRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<Order>(400, "Invalid request data", null));

            var result = await _orderService.AcceptOrderAsync(orderId, request.DeliveryAgentId, cancellationToken);

            if (result.Status == 404)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Reject an order (delivery agent action)
        /// </summary>
        /// <param name="orderId">The ID of the order to reject</param>
        /// <param name="request">Request containing delivery agent ID and rejection reason</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Updated Order entity with rejected status</returns>
        /// <response code="200">Order rejected successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Order not found</response>
        [HttpPost("{orderId}/reject")]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<Order>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Order>>> RejectOrder(
            Guid orderId,
            [FromBody] AcceptRejectOrderRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<Order>(400, "Invalid request data", null));

            var result = await _orderService.RejectOrderAsync(orderId, request.DeliveryAgentId, cancellationToken);

            if (result.Status == 404)
                return NotFound(result);

            return Ok(result);
        }
    }
}
