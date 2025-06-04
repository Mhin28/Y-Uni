using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Services.AccountService;
using Services.Services.AuthenticateService;
using Services.Services.TokenService;
using Services.Services.UserService;
using Services.Services.Validate;

namespace Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IValidate, Validate>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}

