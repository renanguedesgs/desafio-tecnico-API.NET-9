using Application.DTOs;
using Application.UseCases;
using Domain.Abstractions;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace Application.Tests;

public class CreatePatientUseCaseTests
{
    [Fact]
    public void Execute_Should_Create_And_Persist_Patient()
    {
        var repo = new Mock<IPatientRepository>();
        repo.Setup(r => r.Add(It.IsAny<Patient>()));

        var useCase = new CreatePatientUseCase(repo.Object);
        var dto = new PatientDto(0, "Maria", new DateTime(1990, 1, 1), "Hemograma");

        var created = useCase.Execute(dto);

        created.Name.Should().Be("Maria");
        repo.Verify(r => r.Add(It.IsAny<Patient>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Execute_Should_Throw_When_Name_Invalid(string name)
    {
        var repo = new Mock<IPatientRepository>();
        var useCase = new CreatePatientUseCase(repo.Object);
        var dto = new PatientDto(0, name!, new DateTime(1990, 1, 1), "Hemograma");

        var act = () => useCase.Execute(dto);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*nome do paciente*");
    }
}
