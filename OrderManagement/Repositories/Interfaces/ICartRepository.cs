using Orders.DTOs.Response;

namespace Orders.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<CartResponseDto> GetCartAsync(Guid customerId);
        Task<CartResponseDto> AddItemAsync(Guid customerId, Guid restaurantId, Guid productId, string? productName, int quantity, decimal unitPrice, string? specialInstructions);
        Task<CartResponseDto> UpdateItemAsync(Guid customerId, Guid cartItemId, int quantity, string? specialInstructions);
        Task<bool> RemoveItemAsync(Guid customerId, Guid cartItemId);
        Task<bool> ClearCartAsync(Guid customerId);
        Task<CartResponseDto> CheckoutCartAsync(Guid customerId);
        Task<Guid?> GetCartIdAsync(Guid customerId);
    }
}
