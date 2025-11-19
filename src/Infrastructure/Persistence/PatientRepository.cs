using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Entities;

namespace Infrastructure.Persistence;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _db;
    public PatientRepository(AppDbContext db) => _db = db;

    public IEnumerable<Patient> GetAll() =>
        _db.Patients.OrderBy(p => p.Name).ToList();

    public Patient? GetById(int id) =>
        _db.Patients.FirstOrDefault(p => p.Id == id);

    public void Add(Patient patient)
    {
        _db.Patients.Add(patient);
        _db.SaveChanges();
    }
}
