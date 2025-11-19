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
            new Patient { Id = 1, Name = "Maria Silva", BirthDate = new DateTime(1985, 4, 12), LastExam = "Hemograma" },
            new Patient { Id = 2, Name = "João Souza", BirthDate = new DateTime(1990, 7, 23), LastExam = "Raio-X Tórax" },
            new Patient { Id = 3, Name = "Ana Costa", BirthDate = new DateTime(1975, 1, 5), LastExam = "Ultrassom Abdômen" },
            new Patient { Id = 4, Name = "Carlos Lima", BirthDate = new DateTime(2001, 10, 2), LastExam = "Eletrocardiograma" },
        });
        db.SaveChanges();
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();

namespace Api
{
    public partial class Program { }
}
