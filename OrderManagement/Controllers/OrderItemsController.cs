using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.DTOs.RequestDTOs;
using Orders.DTOs.ResponseDTOs;
using Orders.Models;
using Orders.Services.Interfaces;

namespace Orders.Controllers
{
    /// <summary>
    /// Controller for managing order items.
    /// Provides operations to add, update, delete, and retrieve order items.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IExternalProductService _externalProductService;
        private readonly IExternalRestaurantService _externalRestaurantService;
        private readonly IExternalDeliveryService _externalDeliveryService;
        private readonly ILogger<OrderItemsController> _logger;

        /// <summary>
        /// Initializes a new instance of the OrderItemsController.
        /// </summary>
        /// <param name="orderItemService">Service for order item operations</param>
        /// <param name="httpClientFactory">Factory for creating HTTP clients</param>
        /// <param name="externalProductService">Service for product data from external API</param>
        /// <param name="externalRestaurantService">Service for restaurant data from external API</param>
        /// <param name="externalDeliveryService">Service for delivery data from external API</param>
        /// <param name="logger">Logger for diagnostics</param>
        public OrderItemsController(
            IOrderItemService orderItemService,
            IHttpClientFactory httpClientFactory,
            IExternalProductService externalProductService,
            IExternalRestaurantService externalRestaurantService,
            IExternalDeliveryService externalDeliveryService,
            ILogger<OrderItemsController> logger)
        {
            _orderItemService = orderItemService;
            _httpClientFactory = httpClientFactory;
            _externalProductService = externalProductService;
            _externalRestaurantService = externalRestaurantService;
            _externalDeliveryService = externalDeliveryService;
            _logger = logger;
        }

        /// <summary>
        /// Get order item by ID
        /// </summary>
        /// <param name="itemId">The ID of the order item to retrieve</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>OrderItemResponse containing the item details</returns>
        /// <response code="200">Returns the requested order item</response>
        /// <response code="404">Order item not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{itemId}")]
        [ProducesResponseType(typeof(ApiResponse<OrderItemResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<OrderItemResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<OrderItemResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<OrderItemResponse>>> GetItemById(Guid itemId, CancellationToken cancellationToken)
        {
            var items = await _orderItemService.GetItemsByOrderAsync(Guid.Empty, cancellationToken);
            return Ok(items);
        }

        /// <summary>
        /// Add a single order item
        /// </summary>
        /// <param name="request">Request containing order item details</param>
        /// <param name="orderId">The ID of the order to add the item to</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Created OrderItems entity</returns>
        /// <response code="201">Item added successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<OrderItems>>> AddItem(
            [FromBody] CreateOrderItemRequest request,
            [FromQuery] Guid orderId,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<OrderItems>(400, "Invalid request data", null));

            var result = await _orderItemService.AddItemAsync(request, orderId, cancellationToken);

            if (result.Status == 201)
                return CreatedAtAction(nameof(GetItemById), new { itemId = result.Data?.OrderItemId }, result);

            return StatusCode(result.Status, result);
        }


        /// <summary>
        /// Add multiple order items in bulk
        /// </summary>
        /// <param name="request">Request containing list of order items to add</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>List of created OrderItems entities</returns>
        /// <response code="201">Items added successfully</response>
        /// <response code="400">Invalid request data or empty items list</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderItems>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<List<OrderItems>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<List<OrderItems>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<OrderItems>>>> AddItemsBulk(
            [FromBody] BulkCreateOrderItemsRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || request.Items == null || request.Items.Count == 0)
                return BadRequest(new ApiResponse<List<OrderItems>>(400, "Invalid request data or empty items list", null));

            var addedItems = new List<OrderItems>();
            foreach (var item in request.Items)
            {
                var result = await _orderItemService.AddItemAsync(item, request.OrderId, cancellationToken);
                if (result.Data != null)
                    addedItems.Add(result.Data);
            }

            if (addedItems.Count == 0)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<List<OrderItems>>(500, "Failed to add items", null));

            return CreatedAtAction(nameof(AddItem), 
                new ApiResponse<List<OrderItems>>(201, "Items added successfully", addedItems));
        }

        /// <summary>
        /// Get all items for an order
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve items for</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>List of OrderItemResponse objects for the order</returns>
        /// <response code="200">Returns the list of order items</response>
        /// <response code="404">Order not found</response>
        [HttpGet("order/{orderId}")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderItemResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<OrderItemResponse>>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<OrderItemResponse>>>> GetItemsByOrder(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var result = await _orderItemService.GetItemsByOrderAsync(orderId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Update an order item
        /// </summary>
        /// <param name="itemId">The ID of the order item to update</param>
        /// <param name="request">Request containing updated item details</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Updated OrderItems entity</returns>
        /// <response code="200">Item updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Order item not found</response>
        /// <response code="409">Concurrency conflict detected</response>
        [HttpPut("{itemId}")]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<OrderItems>>> UpdateItem(
            Guid itemId,
            [FromBody] UpdateOrderItemRequest request,
            CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<OrderItems>(400, "Invalid request data", null));

            var result = await _orderItemService.UpdateItemAsync(itemId, request, cancellationToken);

            if (result.Status == 404)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete an order item
        /// </summary>
        /// <param name="itemId">The ID of the order item to delete</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Boolean indicating success of deletion</returns>
        /// <response code="200">Item deleted successfully</response>
        /// <response code="404">Order item not found</response>
        [HttpDelete("{itemId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteItem(
            Guid itemId,
            CancellationToken cancellationToken)
        {
            var result = await _orderItemService.DeleteItemAsync(itemId, cancellationToken);

            if (result.Status == 404)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Validate a product with external service
        /// </summary>
        /// <param name="request">Request containing product ID and validation details</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Boolean indicating whether product validation was successful</returns>
        /// <response code="200">Product validation check completed</response>
        /// <response code="400">Product validation failed</response>
        /// <response code="503">External service unavailable</response>
        [HttpPost("validate-product")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateProductWithExternalService(
            [FromBody] ValidateProductRequest request,
            CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");

            var response = await httpClient.PostAsJsonAsync(
                $"/api/products/{request.ProductId}/validate",
                request,
                cancellationToken);

            if (response.IsSuccessStatusCode)
                return Ok(new ApiResponse<bool>(200, "Product validation successful", true));

            return StatusCode(StatusCodes.Status400BadRequest,
                new ApiResponse<bool>(400, "Product validation failed", false));
        }

        /// <summary>
        /// Get product details from external Product service
        /// Demonstrates inter-server communication to fetch product data
        /// </summary>
        /// <param name="productId">The product ID to fetch from external service</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Product details from external service</returns>
        /// <response code="200">Product details retrieved successfully</response>
        /// <response code="404">Product not found in external service</response>
        /// <response code="503">External service unavailable</response>
        [HttpGet("product/{productId}/external")]
        [ProducesResponseType(typeof(ApiResponse<ExternalProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExternalProductDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<ExternalProductDto>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ApiResponse<ExternalProductDto>>> GetProductFromExternalService(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching product from external service. ProductId: {ProductId}", productId);

                // Call external product service to get product details
                var product = await _externalProductService.GetProductDetailsAsync(productId, cancellationToken);

                if (product == null)
                    return NotFound(new ApiResponse<ExternalProductDto>(404, "Product not found in external service", null));

                return Ok(new ApiResponse<ExternalProductDto>(200, "Product retrieved successfully", product));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product from external service");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new ApiResponse<ExternalProductDto>(503, "External service unavailable", null));
            }
        }

        /// <summary>
        /// Validate product availability from external service
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="quantity">Quantity to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Availability status</returns>
        /// <response code="200">Availability check completed</response>
        /// <response code="503">External service unavailable</response>
        [HttpPost("product/{productId}/availability")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ApiResponse<bool>>> CheckProductAvailability(
            Guid productId,
            [FromQuery] int quantity,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Checking product availability. ProductId: {ProductId}, Quantity: {Quantity}",
                    productId, quantity);

                // Call external service to validate availability
                var isAvailable = await _externalProductService.ValidateProductAvailabilityAsync(
                    productId, quantity, cancellationToken);

                return Ok(new ApiResponse<bool>(200, "Availability check completed", isAvailable));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking product availability");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new ApiResponse<bool>(503, "External service unavailable", false));
            }
        }

        /// <summary>
        /// Get product pricing from external service
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Pricing information</returns>
        /// <response code="200">Pricing retrieved successfully</response>
        /// <response code="503">External service unavailable</response>
        [HttpGet("product/{productId}/pricing")]
        [ProducesResponseType(typeof(ApiResponse<ExternalProductPricingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExternalProductPricingDto>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ApiResponse<ExternalProductPricingDto>>> GetProductPricing(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching product pricing. ProductId: {ProductId}", productId);

                // Call external service to get pricing
                var pricing = await _externalProductService.GetProductPricingAsync(productId, cancellationToken);

                if (pricing == null)
                    return NotFound(new ApiResponse<ExternalProductPricingDto>(404, "Pricing not found", null));

                return Ok(new ApiResponse<ExternalProductPricingDto>(200, "Pricing retrieved successfully", pricing));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product pricing");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new ApiResponse<ExternalProductPricingDto>(503, "External service unavailable", null));
            }
        }

        /// <summary>
        /// Get product stock from external service
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stock information</returns>
        /// <response code="200">Stock information retrieved successfully</response>
        /// <response code="503">External service unavailable</response>
        [HttpGet("product/{productId}/stock")]
        [ProducesResponseType(typeof(ApiResponse<ExternalProductStockDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExternalProductStockDto>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ApiResponse<ExternalProductStockDto>>> GetProductStock(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching product stock. ProductId: {ProductId}", productId);

                // Call external service to get stock
                var stock = await _externalProductService.GetProductStockAsync(productId, cancellationToken);

                if (stock == null)
                    return NotFound(new ApiResponse<ExternalProductStockDto>(404, "Stock information not found", null));

                return Ok(new ApiResponse<ExternalProductStockDto>(200, "Stock retrieved successfully", stock));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product stock");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new ApiResponse<ExternalProductStockDto>(503, "External service unavailable", null));
            }
        }

        /// <summary>
        /// Get restaurant details from external service
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Restaurant details</returns>
        /// <response code="200">Restaurant details retrieved successfully</response>
        /// <response code="503">External service unavailable</response>
        [HttpGet("restaurant/{restaurantId}/details")]
        [ProducesResponseType(typeof(ApiResponse<ExternalRestaurantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ExternalRestaurantDto>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ApiResponse<ExternalRestaurantDto>>> GetRestaurantDetails(
            Guid restaurantId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching restaurant details. RestaurantId: {RestaurantId}", restaurantId);

                // Call external service to get restaurant details
                var restaurant = await _externalRestaurantService.GetRestaurantDetailsAsync(restaurantId, cancellationToken);

                if (restaurant == null)
                    return NotFound(new ApiResponse<ExternalRestaurantDto>(404, "Restaurant not found", null));

                return Ok(new ApiResponse<ExternalRestaurantDto>(200, "Restaurant details retrieved successfully", restaurant));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching restaurant details");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new ApiResponse<ExternalRestaurantDto>(503, "External service unavailable", null));
            }
        }

        /// <summary>
        /// Get estimated delivery time from external service
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="deliveryAddressId">Delivery address ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Estimated delivery time in minutes</returns>
        /// <response code="200">Delivery time calculated successfully</response>
        /// <response code="503">External service unavailable</response>
        [HttpGet("delivery/estimated-time")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ApiResponse<int>>> GetEstimatedDeliveryTime(
            [FromQuery] Guid restaurantId,
            [FromQuery] Guid deliveryAddressId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Calculating estimated delivery time. RestaurantId: {RestaurantId}, AddressId: {AddressId}",
                    restaurantId, deliveryAddressId);

                // Call external delivery service
                var estimatedMinutes = await _externalDeliveryService.GetEstimatedDeliveryTimeAsync(
                    restaurantId, deliveryAddressId, cancellationToken);

                return Ok(new ApiResponse<int>(200, "Delivery time calculated successfully", estimatedMinutes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating delivery time");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new ApiResponse<int>(503, "External service unavailable", 0));
            }
        }

        /// <summary>
        /// Check delivery availability from external service
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="deliveryAddressId">Delivery address ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Delivery availability status</returns>
        /// <response code="200">Availability check completed</response>
        /// <response code="503">External service unavailable</response>
        [HttpPost("delivery/availability")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<ApiResponse<bool>>> CheckDeliveryAvailability(
            [FromQuery] Guid restaurantId,
            [FromQuery] Guid deliveryAddressId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Checking delivery availability. RestaurantId: {RestaurantId}, AddressId: {AddressId}",
                    restaurantId, deliveryAddressId);

                // Call external delivery service
                var available = await _externalDeliveryService.IsDeliveryAvailableAsync(
                    restaurantId, deliveryAddressId, cancellationToken);

                return Ok(new ApiResponse<bool>(200, "Availability check completed", available));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking delivery availability");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new ApiResponse<bool>(503, "External service unavailable", false));
            }
        }
    }
}
