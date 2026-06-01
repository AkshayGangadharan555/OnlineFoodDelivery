using Microsoft.EntityFrameworkCore;
using Orders.Models;
using Orders.Authentication;
using Orders.Filters;
using Orders.Services.Interfaces;
using Orders.Services.Implementations;
using Serilog;
using Microsoft.OpenApi.Models;

namespace Orders
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog - Minimal Configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "Logs/orders-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionHandlingFilter>();
                options.Filters.Add<LoggingFilter>();
                options.Filters.Add<ValidationFilter>();
            });

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Order Management API",
                    Version = "v1",
                    Description = "API for managing orders and order items in the Online Food Delivery system",
                    Contact = new OpenApiContact
                    {
                        Name = "Order Management Team",
                        Email = "orders@onlinefooddelivery.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme",
                    In = ParameterLocation.Header,
                    Name = "Authorization"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddDbContext<OrdersContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpClient("ExternalApiClient")
                .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromSeconds(30));

            // Register external service implementations for inter-server communication
            builder.Services.AddScoped<IExternalProductService, ExternalProductService>();
            builder.Services.AddScoped<IExternalRestaurantService, ExternalRestaurantService>();
            builder.Services.AddScoped<IExternalDeliveryService, ExternalDeliveryService>();

            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management API V1");
                options.RoutePrefix = "swagger";
            });

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
