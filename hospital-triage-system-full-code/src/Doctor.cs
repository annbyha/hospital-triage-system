namespace HospitalTriage;

public class Doctor : StaffMember
{
    public string Specialisation { get; set; }

    public Doctor(int id, string name, int age, string specialisation)
        : base(id, name, age, "Doctor") => Specialisation = specialisation;

    public override string GetAssignment() => Specialisation;

    public override string ReceiveAlert(Patient patient) =>
        StoreAlert($"Doctor {Name} ({Specialisation}) received a RED-priority alert for {patient.Name} (ID {patient.PersonID}).");

    public string TreatPatient(Patient patient)
    {
        string message = $"Doctor {Name} is attending to {patient.Name}.";
        Console.WriteLine(message);
        return message;
    }
}
