namespace Orders.Services.Interfaces
{
    /// <summary>
    /// Interface for external product service communication
    /// Handles inter-server communication with Product microservice
    /// </summary>
    public interface IExternalProductService
    {
        /// <summary>
        /// Get product details from external Product service
        /// </summary>
        /// <param name="productId">The product ID to fetch</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Product data from external service</returns>
        Task<ExternalProductDto> GetProductDetailsAsync(Guid productId, CancellationToken cancellationToken);

        /// <summary>
        /// Get multiple products from external Product service
        /// </summary>
        /// <param name="productIds">List of product IDs to fetch</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of products from external service</returns>
        Task<List<ExternalProductDto>> GetProductsAsync(List<Guid> productIds, CancellationToken cancellationToken);

        /// <summary>
        /// Validate product availability with external service
        /// </summary>
        /// <param name="productId">Product ID to validate</param>
        /// <param name="quantity">Quantity to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if product is available, false otherwise</returns>
        Task<bool> ValidateProductAvailabilityAsync(Guid productId, int quantity, CancellationToken cancellationToken);

        /// <summary>
        /// Get product pricing from external service
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Product pricing information</returns>
        Task<ExternalProductPricingDto> GetProductPricingAsync(Guid productId, CancellationToken cancellationToken);

        /// <summary>
        /// Check product stock from external service
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Stock information</returns>
        Task<ExternalProductStockDto> GetProductStockAsync(Guid productId, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Interface for external restaurant service communication
    /// </summary>
    public interface IExternalRestaurantService
    {
        /// <summary>
        /// Get restaurant details from external service
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Restaurant details</returns>
        Task<ExternalRestaurantDto> GetRestaurantDetailsAsync(Guid restaurantId, CancellationToken cancellationToken);

        /// <summary>
        /// Check if restaurant is open
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if restaurant is open</returns>
        Task<bool> IsRestaurantOpenAsync(Guid restaurantId, CancellationToken cancellationToken);

        /// <summary>
        /// Get restaurant delivery zones
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of delivery zones</returns>
        Task<List<DeliveryZoneDto>> GetDeliveryZonesAsync(Guid restaurantId, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Interface for external delivery service communication
    /// </summary>
    public interface IExternalDeliveryService
    {
        /// <summary>
        /// Get estimated delivery time
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="deliveryAddressId">Delivery address ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Estimated delivery time in minutes</returns>
        Task<int> GetEstimatedDeliveryTimeAsync(Guid restaurantId, Guid deliveryAddressId, CancellationToken cancellationToken);

        /// <summary>
        /// Get delivery charges
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="deliveryAddressId">Delivery address ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Delivery charge information</returns>
        Task<DeliveryChargeDto> GetDeliveryChargesAsync(Guid restaurantId, Guid deliveryAddressId, CancellationToken cancellationToken);

        /// <summary>
        /// Check delivery availability
        /// </summary>
        /// <param name="restaurantId">Restaurant ID</param>
        /// <param name="deliveryAddressId">Delivery address ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if delivery available</returns>
        Task<bool> IsDeliveryAvailableAsync(Guid restaurantId, Guid deliveryAddressId, CancellationToken cancellationToken);
    }

    // ======== DTOs for External Service Communication ========

    /// <summary>
    /// External product data transfer object
    /// Received from Product microservice
    /// </summary>
    public class ExternalProductDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; }
        public Guid RestaurantId { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Product pricing information from external service
    /// </summary>
    public class ExternalProductPricingDto
    {
        public Guid ProductId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string Currency { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }

    /// <summary>
    /// Product stock information from external service
    /// </summary>
    public class ExternalProductStockDto
    {
        public Guid ProductId { get; set; }
        public int TotalStock { get; set; }
        public int AvailableStock { get; set; }
        public int ReservedStock { get; set; }
        public DateTime LastRestocked { get; set; }
        public bool IsLowStock { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Restaurant information from external service
    /// </summary>
    public class ExternalRestaurantDto
    {
        public Guid RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public double Rating { get; set; }
        public bool IsOpen { get; set; }
        public string CuisineType { get; set; }
        public TimeSpan EstimatedDeliveryTime { get; set; }
        public decimal DeliveryFee { get; set; }
    }

    /// <summary>
    /// Delivery zone information
    /// </summary>
    public class DeliveryZoneDto
    {
        public Guid ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string PostalCode { get; set; }
        public decimal DeliveryCharge { get; set; }
        public int EstimatedMinutes { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Delivery charge information
    /// </summary>
    public class DeliveryChargeDto
    {
        public decimal BaseCharge { get; set; }
        public decimal DistanceCharge { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalCharge { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
