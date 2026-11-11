namespace HospitalTriage;

public class Nurse : StaffMember
{
    public string Ward { get; set; }

    public Nurse(int id, string name, int age, string ward)
        : base(id, name, age, "Nurse") => Ward = ward;

    public override string GetAssignment() => Ward;

    public override string ReceiveAlert(Patient patient) =>
        StoreAlert($"Nurse {Name} ({Ward}) received a RED-priority alert for {patient.Name} (ID {patient.PersonID}).");

    public string MonitorPatient(Patient patient)
    {
        string message = $"Nurse {Name} is monitoring {patient.Name}.";
        Console.WriteLine(message);
        return message;
    }
}
