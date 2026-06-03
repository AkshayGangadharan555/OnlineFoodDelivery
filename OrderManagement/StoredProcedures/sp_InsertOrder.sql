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
           TaxAmount,
           Discount,
           SubTotal,
           SpecialInstructions,
           Status,
           CreatedAt
       )
       SELECT
           NEWID(),
           @OrderId,
           ProductId,
           RestaurantId,
           Quantity,
           UnitPrice,
           TaxAmount,
           Discount,
           SubTotal,
           SpecialInstructions,
           'Pending',
           GETUTCDATE()
       FROM @Items;

       COMMIT TRANSACTION;
   END TRY
   BEGIN CATCH
       ROLLBACK TRANSACTION;

       THROW;
   END CATCH
END
GO