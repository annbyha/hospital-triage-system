namespace HospitalTriage;

public class Patient
{
    public string PatientId { get; }
    public string Name { get; }
    public int Age { get; }
    public SymptomSeverity Severity { get; }
    public VitalSigns Vitals { get; }

    public Patient(string patientId, string name, int age, SymptomSeverity severity, VitalSigns vitals)
    {
        PatientId = patientId;
        Name = name;
        Age = age;
        Severity = severity;
        Vitals = vitals;
    }

    
    public bool HasSevereSymptoms() => Severity == SymptomSeverity.Severe;
}
