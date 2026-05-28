using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using RestaurantService.Models;
using RestaurantService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
namespace RestaurantService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly RestaurantContext Repo;
        private readonly JWT_HELPER _jwtHelper;
        private readonly IHttpClientFactory _httpClientFactory;

        public RestaurantController(RestaurantContext context, JWT_HELPER jwtHelper, IHttpClientFactory httpClientFactory)
        {
            Repo = context;
            _jwtHelper = jwtHelper;
            _httpClientFactory = httpClientFactory;
        }

        // ⭐ PRIVATE HELPER METHOD: Generates a sequential ID like RES-001, RES-002
        private string GenerateRestaurantId()
        {
            var lastRestaurant = Repo.Restaurants
                .OrderByDescending(r => r.RestaurantId)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastRestaurant != null)
            {
                var numberPart = int.Parse(lastRestaurant.RestaurantId.Substring(4));
                nextNumber = numberPart + 1;
            }

            return $"RES-{nextNumber:D3}";
        }

        [HttpGet("{RestaurantId}/menu")]
        public async Task<IActionResult> GetMenuItemsForRestaurant(string RestaurantId)
        {
            var client = _httpClientFactory.CreateClient("MenuClient"); 
            var response = await client.GetAsync($"api/MenuItems/restaurant/{RestaurantId}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to retrieve menu items");
            }
            var rawMenu = await response.Content.ReadFromJsonAsync<List<ItemDto>>();

            if (rawMenu == null)
            {
                return NotFound("No items found");
            }

            // 4. Client framework filtering execute avvadaniki dynamic reference structure context drop chestunnam
            var menuItems = rawMenu.Select(item => new ItemDto
            {
                ItemName = item.ItemName,
                Category = item.Category,
                Description = item.Description,
                Price = item.Price,
                ImageUrl = item.ImageUrl,
                IsAvailable = item.IsAvailable
            }).ToList();

            // 5. Clean array payload drop array pipeline context final output return
            return Ok(menuItems);
        }
        

        // GET: api/Restaurant
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            return await Repo.Restaurants.ToListAsync();
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetRestaurantByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { message = "Search name cannot be empty" });
            }

            var restaurants = await Repo.Restaurants
                .Where(r => r.RestaurantName.Contains(name))
                .ToListAsync();

            if (restaurants == null || !restaurants.Any())
            {
                return NotFound(new { message = "No restaurants found with that name" });
            }

            return Ok(restaurants);
        }

        // GET: api/Restaurant/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(string id)
        {
            var restaurant = await Repo.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return restaurant;
        }
      
        // POST: api/Restaurant/Register
        [HttpPost("Register")]
        public async Task<ActionResult<Restaurant>> PostRestaurant(Restaurant restaurant)
        {
            if (restaurant == null)
            {
                return BadRequest("Restaurant data is null");
            }

            // ⭐ Calling our custom helper logic here to generate and assign the new unique Restaurant ID!
            restaurant.RestaurantId = GenerateRestaurantId();

            restaurant.VerificationStatus = "Pending"; 
            Repo.Restaurants.Add(restaurant);
            await Repo.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.RestaurantId }, restaurant);
        }
        
        [HttpPost("login")]
       
        public async Task<IActionResult> Login([FromBody] RestaurantLoginDTO loginData)
        {
            var restaurant = await Repo.Restaurants
                .FirstOrDefaultAsync(r => r.Email == loginData.Email && r.PasswordHash == loginData.PasswordHash);

            if (restaurant == null)
            {
                return Unauthorized(new { message = "Invalid Email or Password" });
            }

            var token = _jwtHelper.GenerateToken(restaurant.RestaurantId, restaurant.Email, "Restaurant");
            var expiryMinutes = int.Parse(_jwtHelper._configuration["Jwt:ExpiryMinutes"] ?? "60");

            return Ok(new
            {
                Message = "Login Successful",
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.RestaurantName,
                Token = token,
                Expiry = DateTime.UtcNow.AddMinutes(expiryMinutes)
            });
        }


        [HttpPut("{id}")] 
        [Authorize]
        public async Task<IActionResult> UpdateProfile(string id, [FromBody] RestaurantUpdateDTO updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest("Updated profile data is null");
            }

            // 1. Fetch the actual restaurant entity from the database
            var existingRes = await Repo.Restaurants.FindAsync(id);
            if (existingRes == null)
            {
                return NotFound(new { message = "Restaurant profile not found" });
            }

            // 2. Direct mapping: DTO
            existingRes.RestaurantName = updateDto.RestaurantName;
            existingRes.Category = updateDto.Category;
            existingRes.PhoneNumber = updateDto.PhoneNumber;
            existingRes.Address = updateDto.Address;
            existingRes.City = updateDto.City;
            existingRes.BranchName = updateDto.BranchName;
            existingRes.OpeningTime = updateDto.OpeningTime;
            existingRes.ClosingTime = updateDto.ClosingTime;
            existingRes.Latitude = updateDto.Latitude;
            existingRes.Longitude = updateDto.Longitude;

            // 3. Save the modified changes back to SSMS database
            Repo.Restaurants.Update(existingRes);
            await Repo.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully", updatedData = existingRes });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(string id)
        {
            var restaurant = await Repo.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            Repo.Restaurants.Remove(restaurant);
            await Repo.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("{restaurantId}/orders")]
        // [Authorize] 
        public async Task<IActionResult> GetRestaurantOrders(string restaurantId)
        {
            
            var client = _httpClientFactory.CreateClient("MenuClient");

           
            var response = await client.GetAsync($"api/Orders/restaurant/{restaurantId}");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to retrieve restaurant orders");
            }

            
            var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found for this restaurant yet.");
            }

            
            return Ok(orders);
        }
        [HttpPut("accept-order/{orderId}")]
        public async Task<IActionResult> AcceptOrder(string orderId)
        {
            var client = _httpClientFactory.CreateClient("MenuClient");

            var response = await client.PutAsync($"api/Orders/{orderId}/status?status=Accepted", null);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to connect and accept order from MenuItemsService");
            }

            return Ok("Order accepted by restaurant successfully!");
        }
        [HttpGet("search/menu-filter")]
        public async Task<IActionResult> FilterGlobalMenu([FromQuery] string? name, [FromQuery] string? category, [FromQuery] string? dishType)
        {
            var client = _httpClientFactory.CreateClient("MenuClient");

            // 👈 Added precise absolute path syntax structure match format
            var response = await client.GetAsync($"/api/MenuItems/search/filter?name={name}&category={category}&dishType={dishType}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound(new { message = "No records found matching your search." });
            }

            var items = await response.Content.ReadFromJsonAsync<List<ItemDto>>();
            if (items == null || !items.Any()) return NotFound("No items found.");

            var flatSearchResult = items
                .Select(item => {
                    var res = Repo.Restaurants.Find(item.RestaurantId);
                    return res == null ? null : new
                    {
                        item.ItemName,
                        item.Price,
                        item.Category,
                        item.DishType,
                        item.Description,
                        item.ImageUrl,

                        RestaurantName = res.RestaurantName,
                        RestaurantType = res.Category,
                        Address = res.Address
                    };
                })
                .Where(result => result != null)
                .ToList();

            return Ok(flatSearchResult);
        }
    }
}