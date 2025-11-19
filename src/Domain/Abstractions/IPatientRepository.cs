using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstractions;

public interface IPatientRepository
{
    IEnumerable<Patient> GetAll();
    Patient? GetById(int id);
    void Add(Patient patient);
}