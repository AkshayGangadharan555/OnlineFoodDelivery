using Orders.DTOs.Request;
using Orders.DTOs.Response;

namespace Orders.Services.Interfaces
{
    public interface
      IDeliveryOrderService
    {
        Task<IEnumerable<OrderResponseDto>> GetAvailableOrdersAsync();
        Task<bool> AcceptDeliveryAsync(AssignDeliveryRequestDto request);

        Task<bool> PickupOrderAsync(UpdateOrderStatusRequestDto request, Guid deliveryAgentId);

        Task<bool> DeliverOrderAsync(UpdateOrderStatusRequestDto request, Guid deliveryAgentId);

        Task<IEnumerable<OrderResponseDto>> GetAssignedOrdersAsync(Guid deliveryAgentId);
    }
}
