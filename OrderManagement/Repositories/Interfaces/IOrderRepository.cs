using Orders.DTOs.Request;
using Orders.DTOs.Response;
using System.Collections;

namespace Orders.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        // Customer

        Task<OrderResponseDto>CreateOrderAsync(PlaceOrderRequestDto request);

        Task<OrderResponseDto?>GetOrderByIdAsync(Guid orderId, Guid customerId);

        Task<IEnumerable<OrderResponseDto>>GetCustomerOrdersAsync(Guid customerId);

        Task<bool>CancelOrderAsync(CancelOrderRequestDto request, Guid customerId, string status);

        // Restaurant

        Task<IEnumerable<OrderResponseDto>>GetRestaurantOrdersAsync(Guid restaurantId);

        Task<bool>UpdateOrderStatusAsync(Guid orderId, Guid restaurantId, string status, string? remarks, byte[] rowVersion);

        // Delivery Agent

        Task<IEnumerable<OrderResponseDto>> GetAvailableOrdersAsync();
        Task<bool>AssignDeliveryAsync(AssignDeliveryRequestDto request);

        Task<bool>UpdateDeliveryStatusAsync(Guid orderId, Guid deliveryAgentId,string status, string? remarks, byte[] rowVersion);

        Task<IEnumerable<OrderResponseDto>>GetAssignedOrdersAsync(Guid deliveryAgentId);
    }
}