using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Repositories.Base;
using Repositories.Models;
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

            services.AddScoped<YUniContext>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IExpensesCategoryRepo, ExpensesCategoryRepo>();
            services.AddScoped<IExpenseRepo, ExpenseRepo>();
            services.AddScoped<IPaymentMethodRepo, PaymentMethodRepo>();
            services.AddScoped<IFinancialAccountRepo, FinancialAccountRepo>();
            services.AddScoped<IAssignmentRepo, AssignmentRepo>();
            services.AddScoped<IEventRepo, EventRepo>();
            services.AddScoped<IPriorityLevelRepo, PriorityLevelRepo>();
            services.AddScoped<IEventCategoryRepo, EventCategoryRepo>();
            services.AddScoped<ISubjectRepo, SubjectRepo>();
            services.AddScoped<IPaymentGatewayRepo, PaymentGatewayRepo>();
            services.AddScoped<IDiscountRepo, DiscountRepo>();
			services.AddScoped<IMembershipPlanRepo, MembershipPlanRepo>();
            services.AddScoped<IInvoiceRepo, InvoiceRepo>();
            services.AddScoped<IReminderTemplateRepo, ReminderTemplateRepo>();
			services.AddScoped<IReminderRepo, ReminderRepo>();
            services.AddScoped<IGoalRepo, GoalRepo>();

			return services;
        }
    }
}
