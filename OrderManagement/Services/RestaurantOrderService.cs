using Orders.Constants;
using Orders.DTOs.Request;
using Orders.DTOs.Response;
using Orders.Repositories.Interfaces;
using Orders.Services.Interfaces;

namespace Orders.Services
{
    public class RestaurantOrderService: IRestaurantOrderService
    {
        private readonly IOrderRepository _repository;

        public RestaurantOrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetRestaurantOrdersAsync(Guid restaurantId)
        {
            return await _repository.GetRestaurantOrdersAsync(restaurantId);
        }

        public async Task<bool> AcceptOrderAsync(Guid orderId,Guid restaurantId,byte[] rowVersion)
        {
            return await _repository.UpdateOrderStatusAsync(orderId,restaurantId,OrderStatuses.Confirmed,null,rowVersion);
        }

        public async Task<bool>UpdateOrderStatusAsync(UpdateOrderStatusRequestDto request,Guid restaurantId)
        {
            return await _repository.UpdateOrderStatusAsync(request.OrderId,restaurantId, request.Status, request.Remarks, request.RowVersion);
        }
    }
}