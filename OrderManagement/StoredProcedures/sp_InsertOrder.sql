CREATE PROCEDURE sp_InsertOrderItems
(
   @OrderId UNIQUEIDENTIFIER,
   @Items OrderItemTableType READONLY
)
AS
BEGIN
   SET NOCOUNT ON;

   BEGIN TRY
       BEGIN TRANSACTION;

       INSERT INTO OrderItems
       (
           OrderItemId,
           OrderId,
           ProductId,
           RestaurantId,
           Quantity,
           UnitPrice,
           Discount,
           TaxAmount,
           SpecialInstructions,
           Status,
           CreatedAt,
           LastUpdatedAt,
           CreatedBy
       )
       SELECT
           NEWID(),
           @OrderId,
           ProductId,
           RestaurantId,
           Quantity,
           UnitPrice,
           Discount,
           TaxAmount,
           SpecialInstructions,
           'Pending',
           GETUTCDATE(),
           GETUTCDATE(),
           'System'
       FROM @Items;

       COMMIT TRANSACTION;
   END TRY
   BEGIN CATCH
       ROLLBACK TRANSACTION;

       THROW;
   END CATCH
END
GO