using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext context;

    public PatientRepository(AppDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IEnumerable<Patient> GetAll()
    {
        return context.Patients
            .OrderBy(p => p.Name)
            .ToList();
    }

    public Patient? GetById(int id)
    {
        return context.Patients
            .FirstOrDefault(p => p.Id == id);
    }

    public void Add(Patient patient)
    {
        context.Patients.Add(patient);
        context.SaveChanges();
    }

    public void Update(Patient patient)
    {
        context.Patients.Update(patient);
        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var patient = context.Patients.Find(id);
        if (patient is null) return;

        context.Patients.Remove(patient);
        context.SaveChanges();
    }
}