using Orders.Extensions;
using Orders.Filters;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
        );

        // Controllers
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
            options.Filters.Add<LoggingFilter>();
            options.Filters.Add<ExceptionHandlingFilter>();
        });

        // Register services via extension methods (DB, JWT, Swagger)
        builder.Services.AddCustomDbContext(configuration);
        builder.Services.AddCustomJwt(configuration);
        builder.Services.AddCustomSwagger();
        builder.Services.AddApplicationServices(configuration);

        var app = builder.Build();

        // Use middleware
        app.UseSerilogRequestLogging();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCustomMiddleware();

        app.MapControllers();

        app.Run();
    }
}