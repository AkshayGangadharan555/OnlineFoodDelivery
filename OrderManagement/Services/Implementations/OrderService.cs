using Microsoft.EntityFrameworkCore;
using Orders.DTOs.RequestDTOs;
using Orders.DTOs.ResponseDTOs;
using Orders.Models;
using Orders.Services.Interfaces;

namespace Orders.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly OrdersContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(OrdersContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<Order>> PlaceOrderAsync(CreateOrderRequest request, Guid customerId, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = customerId,
                RestaurantId = request.RestaurantId,
                DeliveryAddressId = request.DeliveryAddressId,
                Status = "Pending",
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = customerId.ToString()
            };

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                var items = request.Items.Select(i => new OrderItems
                {
                    OrderItemId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    ProductId = i.ProductId,
                    RestaurantId = request.RestaurantId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount,
                    TaxAmount = i.TaxAmount,
                    ItemDescription = i.ItemDescription,
                    SpecialInstructions = i.SpecialInstructions,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = customerId.ToString()
                }).ToList();

                _context.OrderItems.AddRange(items);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return new ApiResponse<Order>(201, "Order placed successfully", order);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error placing order");
                return new ApiResponse<Order>(500, "Error placing order", null);
            }
        }

        public async Task<ApiResponse<Order>> UpdateStatusAsync(Guid orderId, string status, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
            if (order == null)
                return new ApiResponse<Order>(404, "Order not found", null);

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<Order>(200, $"Order status updated to {status}", order);
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ApiResponse<Order>(409, "Concurrency conflict detected", null);
            }
        }

        public async Task<ApiResponse<Order>> AcceptOrderAsync(Guid orderId, Guid deliveryAgentId, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
            if (order == null)
                return new ApiResponse<Order>(404, "Order not found", null);

            order.Status = "Accepted";
            order.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<Order>(200, "Order accepted successfully", order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting order");
                return new ApiResponse<Order>(500, "Error accepting order", null);
            }
        }

        public async Task<ApiResponse<Order>> RejectOrderAsync(Guid orderId, Guid deliveryAgentId, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
            if (order == null)
                return new ApiResponse<Order>(404, "Order not found", null);

            order.Status = "Rejected";
            order.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<Order>(200, "Order rejected successfully", order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting order");
                return new ApiResponse<Order>(500, "Error rejecting order", null);
            }
        }

        public async Task<ApiResponse<OrderResponse>> GetOrderAsync(Guid orderId, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

                if (order == null)
                    return new ApiResponse<OrderResponse>(404, "Order not found", null);

                var orderResponse = new OrderResponse
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    RestaurantId = order.RestaurantId,
                    DeliveryAddressId = order.DeliveryAddressId,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt
                };

                return new ApiResponse<OrderResponse>(200, "Order retrieved successfully", orderResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order");
                return new ApiResponse<OrderResponse>(500, "Error retrieving order", null);
            }
        }
    }

}
