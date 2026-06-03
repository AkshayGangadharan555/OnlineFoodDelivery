using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Orders.Config;
using Orders.Data;

namespace Orders.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<OrderDbContext>(options => options.UseSqlServer(conn));
            return services;
        }

        public static IServiceCollection AddCustomJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JwtSettings");
            var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
            var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key ?? string.Empty);

            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register repositories and services
            services.AddHttpClient();
            services.AddScoped<Repositories.Interfaces.IOrderRepository, Repositories.OrderRepository>();
            services.AddScoped<Services.Interfaces.ICustomerOrderService, Services.CustomerOrderService>();
            services.AddScoped<Services.Interfaces.IDeliveryOrderService, Services.DeliveryOrderService>();
            services.AddScoped<Services.Interfaces.IRestaurantOrderService, Services.RestaurantOrderService>();

            // Configure HttpClients for inter-service communication
            var urlsSection = configuration.GetSection("ServiceUrls");
            var restaurantUrl = urlsSection.GetValue<string>("RestaurantService");
            var customerUrl = urlsSection.GetValue<string>("CustomerService");
            var deliveryUrl = urlsSection.GetValue<string>("DeliveryService");

            if (!string.IsNullOrEmpty(restaurantUrl))
            {
                services.AddHttpClient("RestaurantService", client => client.BaseAddress = new Uri(restaurantUrl));
            }

            if (!string.IsNullOrEmpty(customerUrl))
            {
                services.AddHttpClient("CustomerService", client => client.BaseAddress = new Uri(customerUrl));
            }

            if (!string.IsNullOrEmpty(deliveryUrl))
            {
                services.AddHttpClient("DeliveryService", client => client.BaseAddress = new Uri(deliveryUrl));
            }

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Order Management API",
                    Version = "v1"
                });

                // Add JWT Bearer authentication to Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your JWT token"
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
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
