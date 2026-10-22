using System;
namespace HospitalTriage;
// Patient inherits from person
public class Patient : Person
{
    public string PatientId { get; private set; }
    public string Symptom { get; set; }
    public VitalSigns Vitals { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime ArrivalTime { get; private set; }
    public string Status { get; set; }     // "Waiting" or "Treated"
    public string TreatedBy { get; set; }

    public Patient(string id, string name, int age, string symptom, VitalSigns vitals)
        : base(name, age)
    {
        PatientId = id;
        Symptom = symptom;
        Vitals = vitals;
        Priority = PriorityLevel.Green;
        ArrivalTime = DateTime.Now;
        Status = "Waiting";
        TreatedBy = "-";
    }

    public override string GetInfo()
    {
        return PatientId + " - " + Name + " (Age " + Age + ")";
    }
}