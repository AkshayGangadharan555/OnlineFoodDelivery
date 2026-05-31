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
        private readonly ILogger<OrderItemsController> _logger;

        /// <summary>
        /// Initializes a new instance of the OrderItemsController.
        /// </summary>
        /// <param name="orderItemService">Service for order item operations</param>
        /// <param name="httpClientFactory">Factory for creating HTTP clients</param>
        /// <param name="logger">Logger instance for this controller</param>
        public OrderItemsController(
            IOrderItemService orderItemService,
            IHttpClientFactory httpClientFactory,
            ILogger<OrderItemsController> logger)
        {
            _orderItemService = orderItemService;
            _httpClientFactory = httpClientFactory;
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
            _logger.LogInformation("Getting order item with ID: {ItemId}", itemId);

            try
            {
                var items = await _orderItemService.GetItemsByOrderAsync(Guid.Empty, cancellationToken);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order item");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new ApiResponse<OrderItemResponse>(500, "Error retrieving item", null));
            }
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
            _logger.LogInformation("Adding order item to order: {OrderId}", orderId);

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
            _logger.LogInformation("Adding {Count} items in bulk to order: {OrderId}", request.Items.Count, request.OrderId);

            if (!ModelState.IsValid || request.Items == null || request.Items.Count == 0)
                return BadRequest(new ApiResponse<List<OrderItems>>(400, "Invalid request data or empty items list", null));

            try
            {
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

                _logger.LogInformation("Successfully added {Count} items", addedItems.Count);
                return CreatedAtAction(nameof(AddItem), 
                    new ApiResponse<List<OrderItems>>(201, $"Successfully added {addedItems.Count} items", addedItems));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk adding items");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<List<OrderItems>>(500, "Error adding items in bulk", null));
            }
        }

        /// <summary>
        /// Get all items for an order
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve items for</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>List of OrderItemResponse objects for the order</returns>
        /// <response code="200">Returns the list of order items</response>
        /// <response code="404">Order not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("order/{orderId}")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderItemResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<OrderItemResponse>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<OrderItemResponse>>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<OrderItemResponse>>>> GetItemsByOrder(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting items for order: {OrderId}", orderId);

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
        /// <response code="500">Internal server error</response>
        [HttpPut("{itemId}")]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<OrderItems>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<OrderItems>>> UpdateItem(
            Guid itemId,
            [FromBody] UpdateOrderItemRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating order item: {ItemId}", itemId);

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
        /// <response code="500">Internal server error</response>
        [HttpDelete("{itemId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteItem(
            Guid itemId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting order item: {ItemId}", itemId);

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
        /// <response code="500">Internal server error</response>
        [HttpPost("validate-product")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateProductWithExternalService(
            [FromBody] ValidateProductRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating product: {ProductId}", request.ProductId);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");

                var response = await httpClient.PostAsJsonAsync(
                    $"/api/products/{request.ProductId}/validate",
                    request,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Product validation successful for: {ProductId}", request.ProductId);
                    return Ok(new ApiResponse<bool>(200, "Product validation successful", true));
                }
                else
                {
                    _logger.LogWarning("Product validation failed for: {ProductId}", request.ProductId);
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new ApiResponse<bool>(400, "Product validation failed", false));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling external service");
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new ApiResponse<bool>(503, "External service unavailable", false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating product");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<bool>(500, "Error validating product", false));
            }
        }
    }
}
