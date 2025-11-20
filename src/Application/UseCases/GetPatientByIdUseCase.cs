using Domain.Abstractions;
using Domain.Entities;

namespace Application.UseCases;

public class GetPatientByIdUseCase
{
    private readonly IPatientRepository _repo;

    public GetPatientByIdUseCase(IPatientRepository repo)
    {
        _repo = repo;
    }

    public Patient? Execute(int id)
    {
        return _repo.GetById(id);
    }
}
