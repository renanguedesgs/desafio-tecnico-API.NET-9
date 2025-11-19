using Application.DTOs;
using Domain.Abstractions;

namespace Application.UseCases
{
    public class UpdatePatientUseCase
    {
        private readonly IPatientRepository _repo;
        public UpdatePatientUseCase(IPatientRepository repo) => _repo = repo;

        public void Execute(PatientDto dto)
        {
            var patient = _repo.GetById(dto.Id)
            ?? throw new InvalidOperationException("Paciente não encontrado");

            patient.Update(dto.Name, dto.BirthDate, dto.LastExam);

            _repo.Update(patient);
        }
    }
}
