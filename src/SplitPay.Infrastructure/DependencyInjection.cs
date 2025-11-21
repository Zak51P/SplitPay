using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SplitPay.Application.Abstractions;
using SplitPay.Infrastructure.Persistence;
using SplitPay.Infrastructure.Persistence.Repositories;

namespace SplitPay.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is missing");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IGroupsRepository, GroupsRepository>();
        services.AddScoped<IMembersRepository, MembersRepository>();
        services.AddScoped<IExpensesRepository, ExpensesRepository>();

        return services;
    }
}
