using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Abstractions;

namespace Application.UseCases;

public class GetAllPatientsUseCase
{
    private readonly IPatientRepository _repo;
    public GetAllPatientsUseCase(IPatientRepository repo) => _repo = repo;

    public IEnumerable<PatientDto> Execute()
    {
        return _repo.GetAll()
            .Select(p => new PatientDto(p.Id, p.Name, p.BirthDate, p.LastExam));
    }
}
