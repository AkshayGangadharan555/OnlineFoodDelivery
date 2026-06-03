using Orders.DTOs.Request;
using Orders.DTOs.Response;

namespace Orders.Services.Interfaces
{
    public interface
       ICustomerOrderService
    {
        Task<OrderResponseDto> PlaceOrderAsync(PlaceOrderRequestDto request, string token);

        Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId, Guid customerId);

        Task<IEnumerable<OrderResponseDto>> GetCustomerOrdersAsync(Guid customerId);

        Task<bool>CancelOrderAsync(CancelOrderRequestDto request,Guid customerId);
    }
}
