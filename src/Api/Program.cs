using Application.UseCases;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Concurrency;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// MVC + Views
builder.Services.AddControllersWithViews();

// EF Core InMemory
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("AppDb"));

// DI de repositórios e casos de uso
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<GetAllPatientsUseCase>();
builder.Services.AddScoped<CreatePatientUseCase>();
builder.Services.AddScoped<UpdatePatientUseCase>();
builder.Services.AddScoped<DeletePatientUseCase>();

// Redis para lock distribuído
var redisConn = builder.Configuration.GetValue<string>("Redis:Connection", "redis:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
builder.Services.AddSingleton<ILockService, RedisLockService>();

var app = builder.Build();

// Seeding de dados iniciais (pacientes)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Patients.Any())
    {
        db.Patients.AddRange(new[]
        {
            new Patient(1, "Maria Silva", new DateTime(1985, 4, 12), "Hemograma"),
            new Patient(2, "João Souza", new DateTime(1990, 7, 23), "Raio-X Tórax"),
            new Patient(3, "Ana Costa", new DateTime(1975, 1, 5), "Ultrassom Abdômen"),
            new Patient(4, "Carlos Lima", new DateTime(2001, 10, 2), "Eletrocardiograma"),
        });
        db.SaveChanges();
    }
}

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Patients}/{action=Index}/{id?}");

app.Run();

namespace Api
{
    public partial class Program { }
}
