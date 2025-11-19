using Application.DTOs;
using Domain.Abstractions;
using Domain.Entities;
using System;

namespace Application.UseCases
{
    public class CreatePatientUseCase
    {
        private readonly IPatientRepository _repo;

        public CreatePatientUseCase(IPatientRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Patient Execute(PatientDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto), "O DTO do paciente não pode ser nulo.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("O nome do paciente é obrigatório.", nameof(dto.Name));

            if (dto.BirthDate == default)
                throw new ArgumentException("A data de nascimento é obrigatória.", nameof(dto.BirthDate));

            var patient = Patient.Create(dto.Name, dto.BirthDate, dto.LastExam);

            _repo.Add(patient);

            return patient;
        }
    }
}
