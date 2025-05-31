using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}

