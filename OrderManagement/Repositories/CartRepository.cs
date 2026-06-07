using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.DTOs.Response;
using Orders.Models;
using Orders.Repositories.Interfaces;

namespace Orders.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly OrderDbContext _context;

        public CartRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<CartResponseDto> GetCartAsync(Guid customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return new CartResponseDto { CustomerId = customerId };

            return MapToDto(cart);
        }

        public async Task<CartResponseDto> AddItemAsync(Guid customerId, Guid restaurantId, Guid productId, string? productName, int quantity, decimal unitPrice, string? specialInstructions)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    CustomerId = customerId,
                    RestaurantId = restaurantId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = customerId.ToString(),
                    Items = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            var existingItem = cart.Items
                .FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.SpecialInstructions = specialInstructions ?? existingItem.SpecialInstructions;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CartId = cart.CartId,
                    RestaurantId = restaurantId,
                    ProductId = productId,
                    ProductName = productName,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    SpecialInstructions = specialInstructions,
                    CreatedAt = DateTime.UtcNow
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToDto(cart);
        }

        public async Task<CartResponseDto> UpdateItemAsync(Guid customerId, Guid cartItemId, int quantity, string? specialInstructions)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return new CartResponseDto { CustomerId = customerId };

            var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (item == null)
                return MapToDto(cart);

            item.Quantity = quantity;
            item.SpecialInstructions = specialInstructions;
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToDto(cart);
        }

        public async Task<bool> RemoveItemAsync(Guid customerId, Guid cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return false;

            var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
            if (item == null)
                return false;

            _context.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(Guid customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null)
                return false;

            _context.CartItems.RemoveRange(cart.Items);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CartResponseDto> CheckoutCartAsync(Guid customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart == null || cart.Items.Count == 0)
                return new CartResponseDto { CustomerId = customerId };

            return MapToDto(cart);
        }

        public async Task<Guid?> GetCartIdAsync(Guid customerId)
        {
            return await _context.Carts
                .Where(c => c.CustomerId == customerId)
                .Select(c => (Guid?)c.CartId)
                .FirstOrDefaultAsync();
        }

        private static CartResponseDto MapToDto(Cart cart)
        {
            return new CartResponseDto
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId,
                RestaurantId = cart.RestaurantId,
                TotalAmount = cart.Items?.Sum(i => i.Quantity * i.UnitPrice) ?? 0,
                ItemCount = cart.Items?.Count ?? 0,
                CreatedAt = cart.CreatedAt,
                Items = cart.Items?.Select(i => new CartItemResponseDto
                {
                    CartItemId = i.CartItemId,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SpecialInstructions = i.SpecialInstructions,
                    CreatedAt = i.CreatedAt
                }).ToList() ?? new List<CartItemResponseDto>()
            };
        }
    }
}
