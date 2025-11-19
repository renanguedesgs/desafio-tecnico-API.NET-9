namespace Domain.Entities
{
    public class Patient
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public DateTime BirthDate { get; private set; }
        public string LastExam { get; private set; } = string.Empty;

        private Patient() { }

        public Patient(int id, string name, DateTime birthDate, string lastExam)
        {
            Id = id;
            Name = name;
            BirthDate = birthDate;
            LastExam = lastExam ?? string.Empty;
        }

        public static Patient Create(string name, DateTime birthDate, string lastExam)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome inválido.", nameof(name));

            return new Patient(0, name, birthDate, lastExam ?? string.Empty);
        }

        public void Update(string name, DateTime birthDate, string lastExam)
        {
            Name = name;
            BirthDate = birthDate;
            LastExam = lastExam ?? string.Empty;
        }
    }
}
