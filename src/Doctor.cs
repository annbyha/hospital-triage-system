using System;

namespace HospitalTriage;

public class Doctor : StaffMember
{
    public string Specialisation { get; set; }

    public Doctor(
        int id, string name, int age, string specialisation)
        : base(id, name, age, "Doctor")
    {
        Specialisation = specialisation;
    }

    public override void ReceiveAlert(Patient p)
    {
        Console.WriteLine($"Doctor {Name} received an alert for {p.Name}.");
    }

    public void TreatPatient(Patient p)
    {
        Console.WriteLine($"Doctor {Name} is treating {p.Name}.");
    }
}