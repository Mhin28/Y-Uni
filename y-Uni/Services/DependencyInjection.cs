using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Services.AccountService;
using Services.Services.AssignmentService;
using Services.Services.AuditLogService;
using Services.Services.AuthenticateService;
using Services.Services.EventService;
using Services.Services.ExpensesCategoryService;
using Services.Services.ExpenseService;
using Services.Services.FinancialAccountService;
using Services.Services.PaymentMethodService;
using Services.Services.PriorityLevelService;
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
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IExpensesCategoryService, ExpensesCategoryService>();
			services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<IFinancialAccountService, FinancialAccountService>();
            services.AddScoped<IAuditLogsService, AuditLogsService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IPriorityLevelService, PriorityLevelService>();

			return services;
        }
    }
}

