CREATE PROCEDURE sp_UpdateOrderStatus
(
   @OrderId UNIQUEIDENTIFIER,
   @Status NVARCHAR(50),
   @Remarks NVARCHAR(500) = NULL,
   @DeliveryManId UNIQUEIDENTIFIER = NULL,
   @RowVersion VARBINARY(8)
)
AS
BEGIN
   SET NOCOUNT ON;

   BEGIN TRY
       BEGIN TRANSACTION;

       UPDATE Orders
       SET
           Status = @Status,

           StatusRemarks =
               @Remarks,

           DeliveryManId =
               CASE
                   WHEN @DeliveryManId
                   IS NOT NULL
                   THEN @DeliveryManId
                   ELSE DeliveryManId
               END,

           UpdatedAt =
               GETUTCDATE(),

           ActualDeliveryTime =
               CASE
                   WHEN @Status =
                   'Delivered'
                   THEN GETUTCDATE()
                   ELSE ActualDeliveryTime
               END
       WHERE
           OrderId =
               @OrderId
           AND RowVersion =
               @RowVersion;

       IF @@ROWCOUNT = 0
       BEGIN
           THROW 50001,
           'Concurrency conflict or order not found',
           1;
       END

       COMMIT TRANSACTION;
   END TRY
   BEGIN CATCH
       ROLLBACK TRANSACTION;

       THROW;
   END CATCH
END
GO