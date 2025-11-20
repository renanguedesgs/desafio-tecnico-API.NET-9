using Domain.Entities;
using Infrastructure.Persistence;
using System.Linq;

namespace Api.Seed;

public static class DatabaseSeeder
{
    public static void SeedDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!db.Patients.Any())
        {
            db.Patients.AddRange(new[]
            {
                new Patient(1, "Maria Silva", new DateTime(1985, 4, 12), "Hemograma"),
                new Patient(2, "Joao Souza", new DateTime(1990, 7, 23), "Raio-X Torax"),
                new Patient(3, "Ana Costa", new DateTime(1975, 1, 5), "Ultrassom Abdomen"),
                new Patient(4, "Carlos Lima", new DateTime(2001, 10, 2), "Eletrocardiograma"),
            });

            db.SaveChanges();
        }
    }
}
