using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IAuditLogsRepo, AuditLogsRepo>();
			services.AddScoped<IExpensesCategoryRepo, ExpensesCategoryRepo>();
            services.AddScoped<IExpenseRepo, ExpenseRepo>();
			services.AddScoped<IPaymentMethodRepo, PaymentMethodRepo>();
            services.AddScoped<IFinancialAccountRepo, FinancialAccountRepo>();
			// Add other repositories as needed
			// Example: services.AddScoped<IExampleRepo, ExampleRepo>();
			// Register DbContext if needed
			// Example: services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
			return services;
        }
    }
}
