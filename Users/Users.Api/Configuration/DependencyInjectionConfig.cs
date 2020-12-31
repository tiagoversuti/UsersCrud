using Microsoft.Extensions.DependencyInjection;
using Users.Business.Interfaces;
using Users.Business.Notifications;
using Users.Business.Services;
using Users.Data.Context;
using Users.Data.Repositories;

namespace Users.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<INotifier, Notifier>();
            services.AddScoped<UsersContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoginService, LoginService>();

            return services;
        }
    }
}
