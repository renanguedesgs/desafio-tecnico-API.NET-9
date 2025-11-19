using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Application.UnitTests;

public class PatientCrudUnitTests
{
    private AppDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact(DisplayName = "CREATE - Deve adicionar paciente com sucesso")]
    public async Task Create_Patient_Should_Work()
    {
        using var db = CreateInMemoryDb();

        var patient = new Patient(1, "Maria Silva", new DateTime(1985, 4, 12), "Hemograma");
        await db.Patients.AddAsync(patient);
        await db.SaveChangesAsync();

        var saved = await db.Patients.FindAsync(1);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Maria Silva");
        saved.LastExam.Should().Be("Hemograma");
    }

    [Fact(DisplayName = "READ - Deve recuperar paciente existente")]
    public async Task Read_Patient_Should_Work()
    {
        using var db = CreateInMemoryDb();

        var patient = new Patient(1, "João Souza", new DateTime(1990, 7, 23), "Raio-X");
        await db.Patients.AddAsync(patient);
        await db.SaveChangesAsync();

        var saved = await db.Patients.FindAsync(1);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("João Souza");
        saved.LastExam.Should().Be("Raio-X");
    }

    [Fact(DisplayName = "UPDATE - Deve atualizar exame do paciente")]
    public async Task Update_Patient_Should_Work()
    {
        using var db = CreateInMemoryDb();

        var patient = new Patient(1, "Carlos Mendes", new DateTime(1995, 5, 10), "Ultrassom");
        await db.Patients.AddAsync(patient);
        await db.SaveChangesAsync();

        var saved = await db.Patients.FindAsync(1);
        saved!.Update(saved.Name, saved.BirthDate, "Tomografia");
        db.Patients.Update(saved);
        await db.SaveChangesAsync();

        var updated = await db.Patients.FindAsync(1);
        updated!.LastExam.Should().Be("Tomografia");
    }

    [Fact(DisplayName = "DELETE - Deve remover paciente do banco")]
    public async Task Delete_Patient_Should_Work()
    {
        using var db = CreateInMemoryDb();

        var patient = new Patient(1, "Ana Paula", new DateTime(1988, 3, 15), "Hemograma");
        await db.Patients.AddAsync(patient);
        await db.SaveChangesAsync();

        var saved = await db.Patients.FindAsync(1);
        db.Patients.Remove(saved!);
        await db.SaveChangesAsync();

        var deleted = await db.Patients.FindAsync(1);
        deleted.Should().BeNull();
    }
}
