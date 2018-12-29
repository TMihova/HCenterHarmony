using Microsoft.AspNetCore.Builder;

namespace Eventures.Web.Middlewares.MiddlewareExtensions
{
    public static class SeedDataMiddlewareExtensions
    {
        public static IApplicationBuilder UseSeedData(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedDataMiddleware>();
        }
    }
}
