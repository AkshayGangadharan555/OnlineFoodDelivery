using Orders.DTOs.Request;
using Orders.DTOs.Response;

namespace Orders.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(Guid customerId);
        Task<CartResponseDto> AddItemAsync(Guid customerId, Guid restaurantId, AddToCartRequestDto request);
        Task<CartResponseDto> UpdateItemAsync(Guid customerId, Guid cartItemId, UpdateCartItemRequestDto request);
        Task<bool> RemoveItemAsync(Guid customerId, Guid cartItemId);
        Task<bool> ClearCartAsync(Guid customerId);
        Task<OrderResponseDto> CheckoutAsync(Guid customerId, CheckoutRequestDto request, string token);
    }
}
