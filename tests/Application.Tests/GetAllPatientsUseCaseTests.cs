using Application.UseCases;
using Domain.Abstractions;
using Domain.Entities;
using Moq;
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
            new Patient { Id = 1, Name = "A", BirthDate = new DateTime(2000,1,1), LastExam = "X" },
            new Patient { Id = 2, Name = "B", BirthDate = new DateTime(1999,2,2), LastExam = "Y" },
        });

        var useCase = new GetAllPatientsUseCase(repo.Object);
        var result = useCase.Execute().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Name);
        Assert.Equal("Y", result[1].LastExam);
    }
}
