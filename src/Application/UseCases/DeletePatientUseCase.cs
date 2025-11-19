using Domain.Abstractions;

namespace Application.UseCases;

public class DeletePatientUseCase
{
    private readonly IPatientRepository _repo;
    public DeletePatientUseCase(IPatientRepository repo) => _repo = repo;

    public void Execute(int id)
    {
        var patient = _repo.GetById(id) 
        ?? throw new InvalidOperationException("Paciente não encontrado");

        _repo.Delete(id);
    }
}
