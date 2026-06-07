using Orders.DTOs.Request;
using Orders.DTOs.Response;
using Orders.Repositories.Interfaces;
using Orders.Services.Interfaces;

namespace Orders.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICustomerOrderService _orderService;

        public CartService(ICartRepository cartRepository, ICustomerOrderService orderService)
        {
            _cartRepository = cartRepository;
            _orderService = orderService;
        }

        public async Task<CartResponseDto> GetCartAsync(Guid customerId)
        {
            return await _cartRepository.GetCartAsync(customerId);
        }

        public async Task<CartResponseDto> AddItemAsync(Guid customerId, Guid restaurantId, AddToCartRequestDto request)
        {
            return await _cartRepository.AddItemAsync(
                customerId,
                restaurantId,
                request.ProductId,
                request.ProductName,
                request.Quantity,
                request.UnitPrice,
                request.SpecialInstructions);
        }

        public async Task<CartResponseDto> UpdateItemAsync(Guid customerId, Guid cartItemId, UpdateCartItemRequestDto request)
        {
            return await _cartRepository.UpdateItemAsync(customerId, cartItemId, request.Quantity, request.SpecialInstructions);
        }

        public async Task<bool> RemoveItemAsync(Guid customerId, Guid cartItemId)
        {
            return await _cartRepository.RemoveItemAsync(customerId, cartItemId);
        }

        public async Task<bool> ClearCartAsync(Guid customerId)
        {
            return await _cartRepository.ClearCartAsync(customerId);
        }

        public async Task<OrderResponseDto> CheckoutAsync(Guid customerId, CheckoutRequestDto request, string token)
        {
            var cart = await _cartRepository.CheckoutCartAsync(customerId);

            if (cart.Items == null || cart.Items.Count == 0)
                throw new InvalidOperationException("Cart is empty");

            var placeOrderRequest = new PlaceOrderRequestDto
            {
                CustomerId = customerId,
                RestaurantId = request.RestaurantId,
                PaymentAddressId = request.PaymentAddressId,
                DeliveryAddressId = request.DeliveryAddressId,
                Items = cart.Items.Select(i => new OrderItemRequestDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    SpecialInstructions = i.SpecialInstructions
                }).ToList()
            };

            var order = await _orderService.PlaceOrderAsync(placeOrderRequest, token);

            await _cartRepository.ClearCartAsync(customerId);

            return order;
        }
    }
}
