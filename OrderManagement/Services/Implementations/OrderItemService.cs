using Orders.DTOs.RequestDTOs;
using Orders.DTOs.ResponseDTOs;
using Orders.Models;
using Orders.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Orders.Services.Implementations
{
    public class OrderItemService : IOrderItemService
    {
        private readonly OrdersContext _context;
        private readonly ILogger<OrderItemService> _logger;

        public OrderItemService(OrdersContext context, ILogger<OrderItemService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<OrderItems>> AddItemAsync(CreateOrderItemRequest request, Guid orderId, CancellationToken cancellationToken)
        {
            var item = new OrderItems
            {
                OrderItemId = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = request.ProductId,
                RestaurantId = request.RestaurantId,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                Discount = request.Discount,
                TaxAmount = request.TaxAmount,
                ItemDescription = request.ItemDescription,
                SpecialInstructions = request.SpecialInstructions,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _context.OrderItems.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<OrderItems>(201, "Item added successfully", item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to order");
                return new ApiResponse<OrderItems>(500, "Error adding item", null);
            }
        }

        public async Task<ApiResponse<OrderItems>> UpdateItemAsync(Guid itemId, UpdateOrderItemRequest request, CancellationToken cancellationToken)
        {
            var item = await _context.OrderItems.FindAsync(new object[] { itemId }, cancellationToken);
            if (item == null)
                return new ApiResponse<OrderItems>(404, "Order item not found", null);

            item.Quantity = request.Quantity;
            item.Discount = request.Discount;
            item.SpecialInstructions = request.SpecialInstructions;
            item.LastUpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<OrderItems>(200, "Item updated successfully", item);
            }
            catch (DbUpdateConcurrencyException)
            {
                return new ApiResponse<OrderItems>(409, "Concurrency conflict detected", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item");
                return new ApiResponse<OrderItems>(500, "Error updating item", null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteItemAsync(Guid itemId, CancellationToken cancellationToken)
        {
            var item = await _context.OrderItems.FindAsync(new object[] { itemId }, cancellationToken);
            if (item == null)
                return new ApiResponse<bool>(404, "Order item not found", false);

            try
            {
                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync(cancellationToken);
                return new ApiResponse<bool>(200, "Item deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting item");
                return new ApiResponse<bool>(500, "Error deleting item", false);
            }
        }

        public async Task<ApiResponse<List<OrderItemResponse>>> GetItemsByOrderAsync(Guid orderId, CancellationToken cancellationToken)
        {
            try
            {
                var items = await _context.OrderItems
                    .Where(i => i.OrderId == orderId)
                    .Select(i => new OrderItemResponse
                    {
                        OrderItemId = i.OrderItemId,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Discount = i.Discount,
                        TaxAmount = i.TaxAmount,
                        ItemDescription = i.ItemDescription,
                        SpecialInstructions = i.SpecialInstructions,
                        Status = i.Status
                    })
                    .ToListAsync(cancellationToken);

                return new ApiResponse<List<OrderItemResponse>>(200, "Items retrieved successfully", items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items");
                return new ApiResponse<List<OrderItemResponse>>(500, "Error retrieving items", null);
            }
        }
    }

}
