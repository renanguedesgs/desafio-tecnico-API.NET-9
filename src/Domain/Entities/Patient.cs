using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime BirthDate { get; set; }
    public string LastExam { get; set; } = default!;
}