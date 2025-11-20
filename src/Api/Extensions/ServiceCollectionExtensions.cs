using Application.UseCases;
using Domain.Abstractions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllersWithViews();
        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("AppDb"));

        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<GetAllPatientsUseCase>();
        services.AddScoped<CreatePatientUseCase>();
        services.AddScoped<UpdatePatientUseCase>();
        services.AddScoped<DeletePatientUseCase>();
        services.AddScoped<GetPatientByIdUseCase>();
        services.AddScoped<ProcessReportUseCase>();

        var redisConn = config.GetValue<string>("Redis:Connection", "redis:6379");
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
        services.AddSingleton<ILockService, RedisLockService>();

        return services;
    }
}
