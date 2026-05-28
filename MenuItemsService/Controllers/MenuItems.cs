using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MenuItemsService.Models;
using Microsoft.AspNetCore.Authorization;

namespace MenuItemsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly ItemContext _context;

        public MenuItemsController(ItemContext context)
        {
            _context = context;
        }

        //  PRIVATE HELPER METHOD 
        private string GenerateItemId()
        {
            var lastItem = _context.Items
                .OrderByDescending(i => i.ItemId)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastItem != null)
            {
                var numberPart = int.Parse(lastItem.ItemId.Substring(4));
                nextNumber = numberPart + 1;
            }

            return $"ITM-{nextNumber:D3}";
        }

        // 1. GET: api/MenuItems 
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Items.ToListAsync();
            return Ok(items);
        }

        // 2. GET: api/MenuItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(string id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound("Item not found!");
            }

            return Ok(item);
        }

        // 3. GET: api/MenuItems/restaurant/10 
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetItemsByRestaurant(string restaurantId)
        {
            var restaurantItems = await _context.Items
                .Where(i => i.RestaurantId == restaurantId)
                .ToListAsync();

            return Ok(restaurantItems);
        }

        // 4. POST: api/MenuItems 
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateItem([FromBody] Item newItem)
        {
            if (newItem == null)
            {
                return BadRequest("Item data is null");
            }


            newItem.ItemId = GenerateItemId();

            _context.Items.Add(newItem);
            await _context.SaveChangesAsync();


            return Ok(newItem);
        }

        // 5. PUT: api/MenuItems/5 
        // PATCH: api/MenuItems/5 
        // 5. PUT: api/MenuItems/5
        // Item details complete ga update cheyడానికి chala plain logic
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateItem(string id, [FromBody] CreateMenuItemDTO updatedDto)
        {
            if (updatedDto == null)
            {
                return BadRequest("Invalid update data.");
            }

            
            var existingItem = await _context.Items.FindAsync(id);
            if (existingItem == null)
            {
                return NotFound("Item not found to update");
            }

           
            existingItem.ItemName = updatedDto.ItemName;
            existingItem.DishType = updatedDto.DishType;
            existingItem.Description = updatedDto.Description;
            existingItem.Price = updatedDto.Price;
            existingItem.ImageUrl = updatedDto.ImageUrl;
            existingItem.IsAvailable = updatedDto.IsAvailable;

          
            await _context.SaveChangesAsync();

            return Ok(new { message = "Item updated successfully using PUT method!", data = existingItem });
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteItem(string id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound("Item not found to delete");
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Item deleted successfully!");
        }
        [HttpPost("restaurant/{restaurantId}/category/{category}/add-item")]
        [Authorize]
        public async Task<IActionResult> AddMenuItem(string restaurantId, string category, [FromBody] CreateMenuItemDTO newItem) 
        {
            if (newItem == null)
            {
                return BadRequest("Item data is null");
            }

            var item = new Item
            {
                ItemId = "ITM-" + Guid.NewGuid().ToString().Substring(0, 4),
                RestaurantId = restaurantId,
                Category = category, 
                ItemName = newItem.ItemName,
                DishType = newItem.DishType,
                Description = newItem.Description,
                Price = newItem.Price,
                ImageUrl = newItem.ImageUrl,
                IsAvailable = newItem.IsAvailable
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }
        // GET: api/MenuItems/search/filter
        [HttpGet("search/filter")]
        public async Task<IActionResult> SearchItems([FromQuery] string? name, [FromQuery] string? dishType, [FromQuery] string? category)
        {
            var query = _context.Items.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(i => i.ItemName.Contains(name));
            }
            if (!string.IsNullOrEmpty(dishType))
            {
                query = query.Where(i => i.DishType == dishType);
            }
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(i => i.Category == category);
            }

            var filteredItems = await query.ToListAsync();

            if (filteredItems.Count == 0)
            {
                return NotFound("No items matched your search criteria.");
            }

            return Ok(filteredItems);
        }

    }
}