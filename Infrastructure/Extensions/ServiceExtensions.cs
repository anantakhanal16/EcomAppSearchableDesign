using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.ServiceExtensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IProductService, ProductService>(); 
            services.AddScoped<IUtlityServices, UtlitesServices>(); 
            services.AddScoped<IOrderService, OrderService>(); 
            return services;
        }
    }
}