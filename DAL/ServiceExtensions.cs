using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register Identity
            services.AddDefaultIdentity<ApplicationUser>(options =>
                options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }
    }
}
