using Orders.Constants;
using Orders.DTOs.Request;
using Orders.DTOs.Response;
using Orders.Repositories.Interfaces;
using Orders.Services.Interfaces;

namespace Orders.Services
{
    public class CustomerOrderService: ICustomerOrderService
    {
        private readonly IOrderRepository _repository;

        private readonly IHttpClientFactory _httpClientFactory;

        public CustomerOrderService(IOrderRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;

            _httpClientFactory = httpClientFactory;
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(PlaceOrderRequestDto request, string token)
        {
            return await _repository.CreateOrderAsync(request);
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid orderId,Guid customerId)
        {
            return await _repository.GetOrderByIdAsync(orderId,customerId);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetCustomerOrdersAsync(Guid customerId)
        {
            return await _repository.GetCustomerOrdersAsync(customerId);
        }

        public async Task<bool> CancelOrderAsync(CancelOrderRequestDto request, Guid customerId)
        {
            return await _repository.CancelOrderAsync(request,customerId,OrderStatuses.Cancelled);
        }
    }
}