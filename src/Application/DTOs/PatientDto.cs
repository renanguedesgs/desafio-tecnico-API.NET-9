using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public record PatientDto(
        int Id,

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode passar de 100 caracteres")]
        [Display(Name = "Nome completo")]
        string Name,

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [DataType(DataType.Date)]
        [Display(Name = "Data de nascimento")]
        DateTime BirthDate,

        [StringLength(200, ErrorMessage = "O exame não pode passar de 200 caracteres")]
        [Display(Name = "Último exame realizado")]
        string LastExam
    )
    {
        public int Age =>
            DateTime.Today.Year - BirthDate.Year -
            (DateTime.Today.DayOfYear < BirthDate.DayOfYear ? 1 : 0);

        public string ShortInfo =>
            $"{Name} ({Age} anos) - Último exame: {LastExam}";
    }
}
