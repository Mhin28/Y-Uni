using Repositories.Models; // Add this if not already present
using Microsoft.EntityFrameworkCore;
using Services;
using Repositories; // Add this if not already present
using Microsoft.AspNetCore.Routing;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext to the DI container
builder.Services.AddDbContext<YUniContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();

// Register the byte constraint
builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("byte", typeof(ByteRouteConstraint));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
builder.Services
    .AddService(builder.Configuration)
    .AddRepository(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Custom byte route constraint
public class ByteRouteConstraint : IRouteConstraint
{
    public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var value))
        {
            if (value == null)
            {
                return false;
            }

            if (byte.TryParse(value.ToString(), out _))
            {
                return true;
            }
        }

        return false;
    }
}
