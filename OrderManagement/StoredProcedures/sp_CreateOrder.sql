CREATE PROCEDURE sp_CreateOrder
(
   @CustomerId UNIQUEIDENTIFIER,
   @RestaurantId UNIQUEIDENTIFIER,
   @PaymentAddressId UNIQUEIDENTIFIER,
   @DeliveryAddressId UNIQUEIDENTIFIER,
   @TotalAmount DECIMAL(18,2)
)
AS
BEGIN
   SET NOCOUNT ON;

   BEGIN TRY
       BEGIN TRANSACTION;

       DECLARE @OrderId UNIQUEIDENTIFIER;

       SET @OrderId = NEWID();

       INSERT INTO Orders
       (
           OrderId,
           CustomerId,
           RestaurantId,
           DeliveryManId,
           CancelReason,
           StatusRemarks,
           OrderDate,
           Status,
           TotalAmount,
           PaymentAddressId,
           DeliveryAddressId,
           ExpectedDeliveryTime,
           ActualDeliveryTime,
           CreatedAt,
           UpdatedAt,
           CreatedBy
       )
       VALUES
       (
           @OrderId,
           @CustomerId,
           @RestaurantId,
           NULL,
           NULL,
           NULL,
           GETUTCDATE(),
           'Pending',
           @TotalAmount,
           @PaymentAddressId,
           @DeliveryAddressId,
           NULL,
           NULL,
           GETUTCDATE(),
           GETUTCDATE(),
           'System'
       );

       COMMIT TRANSACTION;

       SELECT @OrderId AS OrderId;
   END TRY
   BEGIN CATCH
       ROLLBACK TRANSACTION;

       THROW;
   END CATCH
END
GO