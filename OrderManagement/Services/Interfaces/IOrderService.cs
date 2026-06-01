using Orders.DTOs.RequestDTOs;
using Orders.DTOs.ResponseDTOs;
using Orders.Models;

namespace Orders.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<Order>> PlaceOrderAsync(CreateOrderRequest request, Guid customerId, CancellationToken cancellationToken);
        Task<ApiResponse<Order>> UpdateStatusAsync(Guid orderId, string status, CancellationToken cancellationToken);
        Task<ApiResponse<Order>> AcceptOrderAsync(Guid orderId, Guid deliveryAgentId, CancellationToken cancellationToken);
        Task<ApiResponse<Order>> RejectOrderAsync(Guid orderId, Guid deliveryAgentId, CancellationToken cancellationToken);
        Task<ApiResponse<OrderResponse>> GetOrderAsync(Guid orderId, CancellationToken cancellationToken);
    }
}
