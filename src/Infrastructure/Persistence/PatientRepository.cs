using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _db;

    public PatientRepository(AppDbContext db) => _db = db;

    public IEnumerable<Patient> GetAll() =>
        _db.Patients.OrderBy(p => p.Name).ToList();

    public Patient? GetById(int id)
    {
        return _db.Patients.FirstOrDefault(p => p.Id == id);
    }

    public void Add(Patient patient)
    {
        _db.Patients.Add(patient);
        _db.SaveChanges();
    }

    public void Update(Patient patient)
    {
        _db.Patients.Update(patient);
        _db.SaveChanges();
    }

    public void Delete(int id)
    {
        var patient = _db.Patients.Find(id);
        if (patient is null) return;
        _db.Patients.Remove(patient);
        _db.SaveChanges();
    }
}
