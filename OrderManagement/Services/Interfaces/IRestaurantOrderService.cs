using Orders.DTOs.Request;
using Orders.DTOs.Response;

namespace Orders.Services.Interfaces
{
    public interface
      IRestaurantOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetRestaurantOrdersAsync(Guid restaurantId);

        Task<bool> AcceptOrderAsync(Guid orderId, Guid restaurantId, byte[] rowVersion);

        Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusRequestDto request,Guid restaurantId);
    }
}
