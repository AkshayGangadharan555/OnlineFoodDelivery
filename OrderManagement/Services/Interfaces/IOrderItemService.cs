using Orders.DTOs.ResponseDTOs;
using Orders.DTOs.RequestDTOs;
using Orders.Models;

namespace Orders.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task<ApiResponse<OrderItems>> AddItemAsync(CreateOrderItemRequest request, Guid orderId, CancellationToken cancellationToken);
        Task<ApiResponse<OrderItems>> UpdateItemAsync(Guid itemId, UpdateOrderItemRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<bool>> DeleteItemAsync(Guid itemId, CancellationToken cancellationToken);
        Task<ApiResponse<List<OrderItemResponse>>> GetItemsByOrderAsync(Guid orderId, CancellationToken cancellationToken);
    }
}
