using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Orders.DTOs.ResponseDTOs;
using Orders.Services.Interfaces;

namespace Orders.Services.Implementations
{
    /// <summary>
    /// Implementation for external product service communication
    /// Makes HTTP calls to Product microservice
    /// </summary>
    public class ExternalProductService : IExternalProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExternalProductService> _logger;
        private readonly string _baseUrl;

        /// <summary>
        /// Constructor with dependency injection
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory from DI</param>
        /// <param name="logger">Logger for diagnostics</param>
        /// <param name="configuration">Configuration for external service URL</param>
        public ExternalProductService(
            IHttpClientFactory httpClientFactory,
            ILogger<ExternalProductService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            // Read external service URL from configuration
            _baseUrl = configuration["ExternalServices:ProductServiceUrl"] ?? "https://product-service.example.com";
        }

        /// <summary>
        /// Get single product details from external service
        /// </summary>
        public async Task<ExternalProductDto> GetProductDetailsAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching product details for ProductId: {ProductId}", productId);

                // Create HTTP client with named configuration
                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");

                // Build endpoint URL
                var endpoint = $"{_baseUrl}/api/products/{productId}";

                // Make GET request
                var response = await httpClient.GetAsync(endpoint, cancellationToken);

                // Check if response is successful
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize response to DTO
                    var productData = await response.Content.ReadFromJsonAsync<ExternalProductDto>();
                    _logger.LogInformation("Successfully retrieved product details for ProductId: {ProductId}", productId);
                    return productData;
                }
                else
                {
                    // Log failure
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning(
                        "Failed to fetch product from external service. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error occurred while fetching product {ProductId}", productId);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout while fetching product {ProductId}", productId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching product {ProductId}", productId);
                throw;
            }
        }

        /// <summary>
        /// Get multiple products from external service
        /// </summary>
        public async Task<List<ExternalProductDto>> GetProductsAsync(
            List<Guid> productIds,
            CancellationToken cancellationToken)
        {
            try
            {
                if (productIds == null || productIds.Count == 0)
                {
                    _logger.LogWarning("Empty product ID list provided");
                    return new List<ExternalProductDto>();
                }

                _logger.LogInformation("Fetching {Count} products from external service", productIds.Count);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");

                // Build query string with multiple product IDs
                var queryParams = string.Join("&", productIds.Select(id => $"productIds={id}"));
                var endpoint = $"{_baseUrl}/api/products/bulk?{queryParams}";

                var response = await httpClient.GetAsync(endpoint, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<ExternalProductDto>>();
                    _logger.LogInformation("Successfully retrieved {Count} products", products.Count);
                    return products;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch products. Status: {StatusCode}", response.StatusCode);
                    return new List<ExternalProductDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching multiple products");
                throw;
            }
        }

        /// <summary>
        /// Validate product availability with external service
        /// </summary>
        public async Task<bool> ValidateProductAvailabilityAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Validating product availability. ProductId: {ProductId}, Quantity: {Quantity}",
                    productId, quantity);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");

                var request = new { productId, quantity };
                var endpoint = $"{_baseUrl}/api/products/{productId}/validate-availability";

                var response = await httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    var resultJson = System.Text.Json.JsonDocument.Parse(resultString);
                    bool isAvailable = resultJson.RootElement.TryGetProperty("isAvailable", out var prop) && prop.GetBoolean();
                    _logger.LogInformation("Product availability validation completed. Available: {Available}", isAvailable);
                    return isAvailable;
                }
                else
                {
                    _logger.LogWarning("Product availability validation failed. Status: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating product availability");
                throw;
            }
        }

        /// <summary>
        /// Get product pricing from external service
        /// </summary>
        public async Task<ExternalProductPricingDto> GetProductPricingAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching product pricing for ProductId: {ProductId}", productId);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");
                var endpoint = $"{_baseUrl}/api/products/{productId}/pricing";

                var response = await httpClient.GetAsync(endpoint, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var pricing = await response.Content.ReadFromJsonAsync<ExternalProductPricingDto>();
                    _logger.LogInformation("Successfully retrieved product pricing");
                    return pricing;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch pricing. Status: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product pricing");
                throw;
            }
        }

        /// <summary>
        /// Get product stock from external service
        /// </summary>
        public async Task<ExternalProductStockDto> GetProductStockAsync(
            Guid productId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching product stock for ProductId: {ProductId}", productId);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");
                var endpoint = $"{_baseUrl}/api/products/{productId}/stock";

                var response = await httpClient.GetAsync(endpoint, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var stock = await response.Content.ReadFromJsonAsync<ExternalProductStockDto>();
                    _logger.LogInformation("Successfully retrieved product stock. Available: {Available}", stock.AvailableStock);
                    return stock;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch stock. Status: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product stock");
                throw;
            }
        }
    }

    /// <summary>
    /// Implementation for external restaurant service communication
    /// </summary>
    public class ExternalRestaurantService : IExternalRestaurantService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExternalRestaurantService> _logger;
        private readonly string _baseUrl;

        public ExternalRestaurantService(
            IHttpClientFactory httpClientFactory,
            ILogger<ExternalRestaurantService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _baseUrl = configuration["ExternalServices:RestaurantServiceUrl"] ?? "https://restaurant-service.example.com";
        }

        /// <summary>
        /// Get restaurant details from external service
        /// </summary>
        public async Task<ExternalRestaurantDto> GetRestaurantDetailsAsync(
            Guid restaurantId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching restaurant details for RestaurantId: {RestaurantId}", restaurantId);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");
                var endpoint = $"{_baseUrl}/api/restaurants/{restaurantId}";

                var response = await httpClient.GetAsync(endpoint, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var restaurant = await response.Content.ReadFromJsonAsync<ExternalRestaurantDto>();
                    _logger.LogInformation("Successfully retrieved restaurant details");
                    return restaurant;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch restaurant. Status: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching restaurant details");
                throw;
            }
        }

        /// <summary>
        /// Check if restaurant is open
        /// </summary>
        public async Task<bool> IsRestaurantOpenAsync(
            Guid restaurantId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Checking restaurant status for RestaurantId: {RestaurantId}", restaurantId);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");
                var endpoint = $"{_baseUrl}/api/restaurants/{restaurantId}/status";

                var response = await httpClient.GetAsync(endpoint, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    var resultJson = System.Text.Json.JsonDocument.Parse(resultString);
                    bool isOpen = resultJson.RootElement.TryGetProperty("isOpen", out var prop) && prop.GetBoolean();
                    _logger.LogInformation("Restaurant status retrieved. IsOpen: {IsOpen}", isOpen);
                    return isOpen;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch restaurant status. Status: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking restaurant status");
                throw;
            }
        }

        /// <summary>
        /// Get delivery zones for restaurant
        /// </summary>
        public async Task<List<DeliveryZoneDto>> GetDeliveryZonesAsync(
            Guid restaurantId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching delivery zones for RestaurantId: {RestaurantId}", restaurantId);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");
                var endpoint = $"{_baseUrl}/api/restaurants/{restaurantId}/delivery-zones";

                var response = await httpClient.GetAsync(endpoint, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var zones = await response.Content.ReadFromJsonAsync<List<DeliveryZoneDto>>();
                    _logger.LogInformation("Successfully retrieved {Count} delivery zones", zones.Count);
                    return zones;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch delivery zones. Status: {StatusCode}", response.StatusCode);
                    return new List<DeliveryZoneDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching delivery zones");
                throw;
            }
        }
    }

    /// <summary>
    /// Implementation for external delivery service communication
    /// </summary>
    public class ExternalDeliveryService : IExternalDeliveryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExternalDeliveryService> _logger;
        private readonly string _baseUrl;

        public ExternalDeliveryService(
            IHttpClientFactory httpClientFactory,
            ILogger<ExternalDeliveryService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _baseUrl = configuration["ExternalServices:DeliveryServiceUrl"] ?? "https://delivery-service.example.com";
        }

        /// <summary>
        /// Get estimated delivery time
        /// </summary>
        public async Task<int> GetEstimatedDeliveryTimeAsync(
            Guid restaurantId,
            Guid deliveryAddressId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Calculating estimated delivery time. RestaurantId: {RestaurantId}, AddressId: {AddressId}",
                    restaurantId, deliveryAddressId);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");
                var request = new { restaurantId, deliveryAddressId };
                var endpoint = $"{_baseUrl}/api/delivery/estimated-time";

                var response = await httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    var resultJson = System.Text.Json.JsonDocument.Parse(resultString);
                    int estimatedMinutes = resultJson.RootElement.TryGetProperty("estimatedMinutes", out var prop) ? prop.GetInt32() : 0;
                    _logger.LogInformation("Estimated delivery time: {Minutes} minutes", estimatedMinutes);
                    return estimatedMinutes;
                }
                else
                {
                    _logger.LogWarning("Failed to calculate delivery time. Status: {StatusCode}", response.StatusCode);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating delivery time");
                throw;
            }
        }

        /// <summary>
        /// Get delivery charges
        /// </summary>
        public async Task<DeliveryChargeDto> GetDeliveryChargesAsync(
            Guid restaurantId,
            Guid deliveryAddressId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching delivery charges. RestaurantId: {RestaurantId}, AddressId: {AddressId}",
                    restaurantId, deliveryAddressId);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");
                var request = new { restaurantId, deliveryAddressId };
                var endpoint = $"{_baseUrl}/api/delivery/charges";

                var response = await httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var charges = await response.Content.ReadFromJsonAsync<DeliveryChargeDto>();
                    _logger.LogInformation("Successfully calculated delivery charges: {Total}", charges.TotalCharge);
                    return charges;
                }
                else
                {
                    _logger.LogWarning("Failed to fetch delivery charges. Status: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching delivery charges");
                throw;
            }
        }

        /// <summary>
        /// Check delivery availability
        /// </summary>
        public async Task<bool> IsDeliveryAvailableAsync(
            Guid restaurantId,
            Guid deliveryAddressId,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Checking delivery availability. RestaurantId: {RestaurantId}, AddressId: {AddressId}",
                    restaurantId, deliveryAddressId);

                var httpClient = _httpClientFactory.CreateClient("ExternalApiClient");
                var request = new { restaurantId, deliveryAddressId };
                var endpoint = $"{_baseUrl}/api/delivery/availability";

                var response = await httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    var resultJson = System.Text.Json.JsonDocument.Parse(resultString);
                    bool available = resultJson.RootElement.TryGetProperty("available", out var prop) && prop.GetBoolean();
                    _logger.LogInformation("Delivery availability: {Available}", available);
                    return available;
                }
                else
                {
                    _logger.LogWarning("Failed to check delivery availability. Status: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking delivery availability");
                throw;
            }
        }
    }
}
