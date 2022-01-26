using Microsoft.AspNetCore.Builder;
namespace Middleware
{
    public static class MyAppExtensions
    {
        // Mở rộng cho IApplicationBuilder phương thức UseCheckAccess
        public static IApplicationBuilder UseCheckAccess(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckAcessMiddleware>();
        }
    }

}
