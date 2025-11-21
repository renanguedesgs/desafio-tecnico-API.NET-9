using Application.UseCases;
using Domain.Abstractions;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

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

    #region Teste da criação de um novo paciente
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
    #endregion

    #region Teste da edição de um paciente
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
    #endregion

    #region Teste da exclusão de um paciente
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
    #endregion//Teste do relatório com lock distribuído

    #region Teste do relatório com lock distribuído
    private class FakeLockService : ILockService
    {
        private bool isLocked = false;
        private bool allowRelease = true;

        public void BlockRelease() => allowRelease = false;
        public void EnableRelease() => allowRelease = true;

        public bool TryAcquire(string key, TimeSpan ttl)
        {
            if (isLocked) return false;
            isLocked = true;
            return true;
        }

        public void Release(string key)
        {
            if (allowRelease)
                isLocked = false;
        }

        public Task<bool> TryAcquireAsync(string key, TimeSpan ttl, CancellationToken ct = default)
            => Task.FromResult(TryAcquire(key, ttl));

        public Task ReleaseAsync(string key, CancellationToken ct = default)
        {
            Release(key);
            return Task.CompletedTask;
        }
    }

    [Fact(DisplayName = "processamento-relatorio - Deve executar apenas uma chamada e bloquear as demais")]
    public void ProcessReport_Should_Respect_Lock()
    {
        var lockService = new FakeLockService();
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ProcessReportUseCase>.Instance;
        var useCase = new ProcessReportUseCase(lockService, logger);

        lockService.BlockRelease();

        var result1 = useCase.Execute();
        result1.Should().Be("Processo concluído");

        var result2 = useCase.Execute();
        result2.Should().Be("Recurso ocupado. Tente novamente mais tarde.");

        lockService.EnableRelease();
        lockService.Release("report");

        var result3 = useCase.Execute();
        result3.Should().Be("Processo concluído");
    }

    [Fact(DisplayName = "processamento-relatorio - Deve bloquear chamadas concorrentes reais")]
    public async Task ProcessReport_Should_Block_Concurrent_Calls()
    {
        var lockService = new FakeLockService();
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ProcessReportUseCase>.Instance;
        var useCase = new ProcessReportUseCase(lockService, logger);

        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => useCase.Execute()))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        results.Count(r => r == "Processo concluído").Should().Be(1);
        results.Count(r => r == "Recurso ocupado. Tente novamente mais tarde.").Should().Be(9);
    }

    #endregion

}