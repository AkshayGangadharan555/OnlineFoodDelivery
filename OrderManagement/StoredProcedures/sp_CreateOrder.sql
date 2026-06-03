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
           PaymentAddressId,
           DeliveryAddressId,
           DeliveryAgentId,
           TotalAmount,
           Status,
           StatusRemarks,
           CancellationReason,
           OrderDate,
           CreatedAt
       )
       VALUES
       (
           @OrderId,
           @CustomerId,
           @RestaurantId,
           @PaymentAddressId,
           @DeliveryAddressId,
           NULL,
           @TotalAmount,
           'Pending',
           NULL,
           NULL,
           GETUTCDATE(),
           GETUTCDATE()
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