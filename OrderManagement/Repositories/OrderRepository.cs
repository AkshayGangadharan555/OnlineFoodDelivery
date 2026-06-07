using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Orders.Constants;
using Orders.Data;
using Orders.DTOs.Request;
using Orders.DTOs.Response;
using Orders.Repositories.Interfaces;
using System.Data;

namespace Orders.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IConfiguration _configuration;
        private readonly OrderDbContext _context;

        public OrderRepository(IConfiguration configuration, OrderDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        // CREATE ORDER
               public async Task<OrderResponseDto> CreateOrderAsync(PlaceOrderRequestDto request)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            try
            {
                var orderId = await connection.QuerySingleAsync<Guid>(
                    "sp_CreateOrder",
                    new
                    {
                        request.CustomerId,
                        request.RestaurantId,
                        request.PaymentAddressId,
                        request.DeliveryAddressId,
                        TotalAmount = 0
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                var orderItemsTable = new DataTable();
                orderItemsTable.Columns.Add("ProductId", typeof(Guid));
                orderItemsTable.Columns.Add("RestaurantId", typeof(Guid));
                orderItemsTable.Columns.Add("Quantity", typeof(int));
                orderItemsTable.Columns.Add("UnitPrice", typeof(decimal));
                orderItemsTable.Columns.Add("TaxAmount", typeof(decimal));
                orderItemsTable.Columns.Add("Discount", typeof(decimal));
                orderItemsTable.Columns.Add("SpecialInstructions", typeof(string));

                foreach (var item in request.Items)
                {
                    decimal unitPrice = 0.00m;
                    decimal taxAmount = 0.00m;
                    decimal discount = 0.00m;

                    orderItemsTable.Rows.Add(
                        item.ProductId,
                        request.RestaurantId,
                        item.Quantity,
                        unitPrice,
                        taxAmount,
                        discount,
                        (object)item.SpecialInstructions ?? DBNull.Value
                    );
                }

                await connection.ExecuteAsync(
                    "sp_InsertOrderItems",
                    new
                    {
                        OrderId = orderId,
                        Items = orderItemsTable.AsTableValuedParameter("OrderItemTableType")
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                transaction.Commit();

                return new OrderResponseDto
                {
                    OrderId = orderId,
                    CustomerId = request.CustomerId,
                    RestaurantId = request.RestaurantId,
                    Status = "Pending"
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        
        // GET ORDER
        public async Task<OrderResponseDto?>GetOrderByIdAsync(Guid orderId,Guid customerId)
        {
            return await _context.Orders
                .Include(o =>o.Items)
                .Where(o => o.OrderId == orderId && o.CustomerId == customerId)
                .Select(o => new OrderResponseDto
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        RestaurantId = o.RestaurantId,
                        Status = o.Status,
                        TotalAmount = o.TotalAmount
                    }).FirstOrDefaultAsync();
        }

        // CUSTOMER ORDERS
        public async Task<IEnumerable<OrderResponseDto>>GetCustomerOrdersAsync(Guid customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Select(o => new OrderResponseDto{
                        OrderId =o.OrderId,
                        Status =o.Status,
                        TotalAmount =o.TotalAmount
                    }).ToListAsync();
        }

        // CANCEL ORDER
        public async Task<bool>CancelOrderAsync(CancelOrderRequestDto request,Guid customerId,string status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == request.OrderId && o.CustomerId == customerId);
            if (order == null)
                return false;
            order.Status = status;
            order.CancelReason = request.CancelReason;
            await _context.SaveChangesAsync();
            return true;
        }

        //RESTAURANT ORDER

        public async Task<IEnumerable<OrderResponseDto>> GetRestaurantOrdersAsync(Guid restaurantId)
        {
            return await _context.Orders.Include(o => o.Items).Where(o => o.RestaurantId == restaurantId)
                                  .Select(o => new OrderResponseDto{ 
                                      OrderId = o.OrderId,
                                      CustomerId = o.CustomerId,
                                      RestaurantId = o.RestaurantId,
                                      TotalAmount = o.TotalAmount,
                                      Status = o.Status,
                                      OrderDate = o.OrderDate
                                  }).ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, Guid restaurantId, string status,string? remarks, byte[] rowVersion) 
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId && o.RestaurantId == restaurantId); 
            if (order == null)
                return false;

            _context.Entry(order).Property("RowVersion").OriginalValue = rowVersion;
            order.Status = status;
            order.StatusRemarks = remarks;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        //DELIVERY MAN GET ORDER STATUS
        public async Task<IEnumerable<OrderResponseDto>> GetAvailableOrdersAsync()
        {
            return await _context.Orders.Where(o => o.Status == OrderStatuses.Ready && o.DeliveryManId == null)
                                 .Select(o => new OrderResponseDto
                                 {
                                     OrderId = o.OrderId,
                                     RestaurantId = o.RestaurantId,
                                     TotalAmount = o.TotalAmount,
                                     Status = o.Status,
                                 }).ToListAsync();
        }

        public async Task<bool>AssignDeliveryAsync(AssignDeliveryRequestDto request)
        {
            var order =await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == request.OrderId);

            if (order == null)
                return false;

            _context.Entry(order).Property("RowVersion").OriginalValue =request.RowVersion;
            order.DeliveryManId =request.DeliveryManId;
            order.Status =OrderStatuses.Assigned;
            order.UpdatedAt =DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool>UpdateDeliveryStatusAsync(Guid orderId,Guid deliveryAgentId,string status,string? remarks,byte[] rowVersion)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o =>o.OrderId ==orderId && o.DeliveryManId == deliveryAgentId);

            if (order == null)
                return false;

            _context.Entry(order).Property("RowVersion").OriginalValue =rowVersion;

            order.Status = status;
            order.StatusRemarks = remarks;
            order.UpdatedAt = DateTime.UtcNow;

            if (status == OrderStatuses.Delivered)
            {
                order.ActualDeliveryTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<OrderResponseDto>>GetAssignedOrdersAsync(Guid deliveryAgentId)
        {
            return await _context.Orders
                .Where(o => o.DeliveryManId == deliveryAgentId)
                .Select(o => new OrderResponseDto {
                        OrderId =o.OrderId,
                        RestaurantId =o.RestaurantId,
                        Status =o.Status,
                        TotalAmount =o.TotalAmount
                    }).ToListAsync();
        }

    }
}
