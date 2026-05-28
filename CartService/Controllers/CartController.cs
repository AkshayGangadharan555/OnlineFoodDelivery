using CartService.Models;
using CustomerService.Models;
using MenuItems.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartItemsContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CartController(
            CartItemsContext context,
            HttpClient httpClient,
            IConfiguration config)
        {
            _context = context;
            _httpClient = httpClient;
            _config = config;
        }

        
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(string customerId, int menuItemId, int quantity)
        {
            var menuApi = _config["MenuItemService:BaseUrl"];
            var res = await _httpClient.GetAsync($"{menuApi}/api/menuitem/{menuItemId}");

            if (!res.IsSuccessStatusCode)
                return BadRequest("Menu item not found");

            var menu = await res.Content.ReadFromJsonAsync<MenuItem>();

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CustomerID == customerId &&
                    c.MenuItemID == menuItemId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.UpdatedDate = DateTime.Now;
            }
            else
            {
                cartItem = new CartItems
                {
                    CustomerID = customerId,
                    MenuItemID = menuItemId,
                    Quantity = quantity,
                    Price = menu.Price,
                    AddedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Item added",
                cartItemId = cartItem.CartItemID
            });
        }
       
       

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCart(string customerId)
        {
            var menuApi = _config["MenuItemService:BaseUrl"];

            var cartItems = await _context.CartItems
                .Where(c => c.CustomerID == customerId)
                .ToListAsync();

            var result = new List<CartItemDTO>();
            decimal totalAmount = 0;

            foreach (var item in cartItems)
            {
                var res = await _httpClient.GetAsync($"{menuApi}/api/menuitem/{item.MenuItemID}");
                if (!res.IsSuccessStatusCode) continue;

                var menu = await res.Content.ReadFromJsonAsync<MenuItem>();

                var itemTotal = menu.Price * item.Quantity;
                totalAmount += itemTotal;

                result.Add(new CartItemDTO
                {
                    CartItemID = item.CartItemID,
                    ItemName = menu.Name,
                    ItemPrice = menu.Price,
                    Quantity = item.Quantity,
                    TotalPrice = itemTotal,
                    Status = menu.Status
                });
            }

            return Ok(new
            {
                CustomerID = customerId,
                Items = result,
                TotalAmount = totalAmount
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCartItems()
        {
            var menuApi = _config["MenuItemService:BaseUrl"];

            var cartItems = await _context.CartItems.ToListAsync();
            var result = new List<CartItemDTO>();

            foreach (var item in cartItems)
            {
                var res = await _httpClient.GetAsync($"{menuApi}/api/menuitem/{item.MenuItemID}");
                if (!res.IsSuccessStatusCode) continue;

                var menu = await res.Content.ReadFromJsonAsync<MenuItem>();

                result.Add(new CartItemDTO
                {
                    CartItemID = item.CartItemID,
                    ItemName = menu.Name,
                    ItemPrice = menu.Price,
                    Quantity = item.Quantity,
                    TotalPrice = menu.Price * item.Quantity,
                    Status = menu.Status
                });
            }

            return Ok(result);
        }


        [HttpPut("increase")]
        public async Task<IActionResult> Increase(string customerId, string itemName)
        {
            var menuApi = _config["MenuItemService:BaseUrl"];

            // ✅ Get menu item by name
            var menuRes = await _httpClient.GetAsync($"{menuApi}/api/menuitem/byname/{itemName}");

            if (!menuRes.IsSuccessStatusCode)
                return BadRequest("Menu item not found");

            var menu = await menuRes.Content.ReadFromJsonAsync<MenuItem>();

            // ✅ Find cart item using CustomerID + MenuItemID
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CustomerID == customerId &&
                    c.MenuItemID == menu.ItemID);

            if (cartItem == null)
                return NotFound("Cart item not found");

            // ✅ Increase quantity
            cartItem.Quantity += 1;
            cartItem.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Quantity increased",
                customerId,
                itemName,
                newQuantity = cartItem.Quantity
            });
        }
        [HttpPut("decrease")]
        public async Task<IActionResult> Decrease(string customerId, string itemName)
        {
            var menuApi = _config["MenuItemService:BaseUrl"];

            // ✅ Get menu item
            var menuRes = await _httpClient.GetAsync($"{menuApi}/api/menuitem/byname/{itemName}");

            if (!menuRes.IsSuccessStatusCode)
                return BadRequest("Menu item not found");

            var menu = await menuRes.Content.ReadFromJsonAsync<MenuItem>();

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CustomerID == customerId &&
                    c.MenuItemID == menu.ItemID);

            if (cartItem == null)
                return NotFound("Cart item not found");

            // ✅ Decrease logic
            cartItem.Quantity -= 1;

            if (cartItem.Quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.UpdatedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Quantity decreased",
                customerId,
                itemName,
                newQuantity = cartItem.Quantity > 0 ? cartItem.Quantity : 0
            });
        }
        
        [HttpDelete("remove")]
        public async Task<IActionResult> DeleteItem(string customerId, string itemName)
        {
            var menuApi = _config["MenuItemService:BaseUrl"];

            var menuRes = await _httpClient.GetAsync($"{menuApi}/api/menuitem/byname/{itemName}");

            if (!menuRes.IsSuccessStatusCode)
                return BadRequest("Menu item not found");

            var menu = await menuRes.Content.ReadFromJsonAsync<MenuItem>();

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CustomerID == customerId &&
                    c.MenuItemID == menu.ItemID);

            if (item == null)
                return NotFound("Cart item not found");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Item removed");
        }

        [HttpDelete("clear/{customerId}")]
        public async Task<IActionResult> ClearCart(string customerId)
        {
            var items = await _context.CartItems
                .Where(c => c.CustomerID == customerId)
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok("Cart cleared");
        }
    }
}
