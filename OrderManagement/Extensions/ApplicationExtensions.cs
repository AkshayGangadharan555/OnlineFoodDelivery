using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Orders.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication UseCustomMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
