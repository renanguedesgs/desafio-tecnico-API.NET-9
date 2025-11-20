namespace Application.UseCases
{
    using Application.DTOs;
    using Domain.Abstractions;
    using Domain.Entities;

    public class CreatePatientUseCase
    {
        private readonly IPatientRepository repository;

        public CreatePatientUseCase(IPatientRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Patient Execute(PatientDto dto)
        {
            Validate(dto);

            var patient = Patient.Create(dto.Name, dto.BirthDate, dto.LastExam);

            repository.Add(patient);

            return patient;
        }

        private static void Validate(PatientDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Nome do paciente é obrigatório.", nameof(dto.Name));

            if (dto.BirthDate == default)
                throw new ArgumentException("Data de nascimento é obrigatória.", nameof(dto.BirthDate));
        }
    }
}
