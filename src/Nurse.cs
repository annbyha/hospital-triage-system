using System;

namespace HospitalTriage;

public class Nurse : StaffMember
{
    public string Ward { get; set; }

    public Nurse(
        int id, string name, int age, string ward)
        : base(id, name, age, "Nurse")
    {
        Ward = ward;
    }

    public override void ReceiveAlert(Patient p)
    {
        Console.WriteLine($"Nurse {Name} received an alert for {p.Name}.");
    }

    public void MonitorPatient(Patient p)
    {
        Console.WriteLine($"Nurse {Name} is monitoring {p.Name}.");
    }
}