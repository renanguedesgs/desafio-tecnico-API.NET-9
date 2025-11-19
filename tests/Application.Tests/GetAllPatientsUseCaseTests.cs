using Application.UseCases;
using Domain.Abstractions;
using Domain.Entities;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Application.Tests;

public class GetAllPatientsUseCaseTests
{
    [Fact]
    public void Execute_ShouldMapEntitiesToDtos()
    {
        var repo = new Mock<IPatientRepository>();
        repo.Setup(r => r.GetAll()).Returns(new[]
        {
            new Patient(1, "A", new DateTime(2000,1,1), "X"),
            new Patient(2, "B", new DateTime(1999,2,2), "Y"),
        });

        var useCase = new GetAllPatientsUseCase(repo.Object);
        var result = useCase.Execute().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Name);
        Assert.Equal("Y", result[1].LastExam);
    }
}
