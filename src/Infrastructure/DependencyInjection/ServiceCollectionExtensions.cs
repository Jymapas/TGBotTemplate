using Application.Interfaces;
using Infrastructure.Database;
using Infrastructure.Options;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, BotOptions botOptions, IEnumerable<long> adminIds)
    {
        services.AddSingleton(botOptions);

        services.AddDbContextFactory<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={botOptions.DbPath}");
        });

        services.AddScoped<ISessionService, SessionService>();
        services.AddSingleton<IRoleService>(_ => new RoleService(adminIds));

        return services;
    }
}
