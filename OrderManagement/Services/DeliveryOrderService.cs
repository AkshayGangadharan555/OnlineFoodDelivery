using Orders.Constants;
using Orders.DTOs.Request;
using Orders.DTOs.Response;
using Orders.Repositories.Interfaces;
using Orders.Services.Interfaces;

namespace Orders.Services
{
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly IOrderRepository _repository;

        public DeliveryOrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AcceptDeliveryAsync(AssignDeliveryRequestDto request)
        {
            return await _repository.AssignDeliveryAsync(request);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAvailableOrdersAsync()
        {
            return await _repository.GetAvailableOrdersAsync();
        }

        public async Task<bool> PickupOrderAsync(UpdateOrderStatusRequestDto request, Guid deliveryAgentId)
        {
            return await _repository.UpdateDeliveryStatusAsync(request.OrderId, deliveryAgentId, OrderStatuses.PickedUp, request.Remarks, request.RowVersion);
        }

        public async Task<bool> DeliverOrderAsync(UpdateOrderStatusRequestDto request, Guid deliveryAgentId)
        {
            return await _repository.UpdateDeliveryStatusAsync(request.OrderId, deliveryAgentId, OrderStatuses.Delivered, request.Remarks, request.RowVersion);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAssignedOrdersAsync(Guid deliveryAgentId)
        {
            return await _repository.GetAssignedOrdersAsync(deliveryAgentId);
        }
    }
}