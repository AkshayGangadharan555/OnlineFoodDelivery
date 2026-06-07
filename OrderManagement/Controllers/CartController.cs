using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.DTOs.Request;
using Orders.DTOs.Response;
using Orders.Services.Interfaces;
using System.Security.Claims;

namespace Orders.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _cartService.GetCartAsync(customerId);
            return Ok(new ApiResponse<CartResponseDto>(200, "Cart retrieved successfully", result));
        }

        [HttpPost("add/{restaurantId}")]
        public async Task<IActionResult> AddItem(Guid restaurantId, AddToCartRequestDto request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _cartService.AddItemAsync(customerId, restaurantId, request);
            return Ok(new ApiResponse<CartResponseDto>(200, "Item added to cart", result));
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateItem(Guid cartItemId, UpdateCartItemRequestDto request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _cartService.UpdateItemAsync(customerId, cartItemId, request);

            if (result.Items == null || !result.Items.Any())
                return NotFound(new ApiResponse<CartResponseDto>(404, "Item not found", result));

            return Ok(new ApiResponse<CartResponseDto>(200, "Cart item updated", result));
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _cartService.RemoveItemAsync(customerId, cartItemId);

            if (!result)
                return NotFound(new ApiResponse<string>(404, "Item not found", ""));

            return Ok(new ApiResponse<string>(200, "Item removed from cart", ""));
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _cartService.ClearCartAsync(customerId);

            if (!result)
                return NotFound(new ApiResponse<string>(404, "Cart not found", ""));

            return Ok(new ApiResponse<string>(200, "Cart cleared", ""));
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(CheckoutRequestDto request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                var order = await _cartService.CheckoutAsync(customerId, request, token);
                return Ok(new ApiResponse<OrderResponseDto>(200, "Order placed successfully", order));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>(400, ex.Message, ""));
            }
        }
    }
}
