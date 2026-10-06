using ExpenseTracker.Infrastructure.Persistence;
using ExpenseTracker.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ExpenseTracker.Application.Common.Interfaces;
using ExpenseTracker.Application.Auth;
using ExpenseTracker.Infrastructure.Identity;

namespace ExpenseTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<AuditableEntityInterceptor>();

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                options.UseSqlServer(connectionString);

                options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>());
            });

            // When something asks for IApplicationDbContext, give it the same AppDbContext instance
            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

            // ── Authentication ──────────────────────────────────────
            services.AddOptions<JwtSettings>().Bind(configuration.GetSection(JwtSettings.SectionName));

            services.AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
