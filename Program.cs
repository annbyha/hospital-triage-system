using System;

namespace HospitalTriage;

public class Program
{
    public static void Main(string[] args)
    {
        // Create triage system
        SevereSymptomRules rules = new SevereSymptomRules();
        TriageEngine engine = new TriageEngine(rules);

        // Register staff
        Nurse nurse = new Nurse(101, "Siti", 30, "Emergency Ward");
        Doctor doctor = new Doctor(102, "Aiman", 40, "Emergency Medicine");

        engine.RegisterStaff(nurse);
        engine.RegisterStaff(doctor);

        // Create sample patient
        VitalSigns vitals = new VitalSigns(130, 120);

        Patient patient = new Patient(
            1, "Ahmad", 45, vitals);

        patient.AddSymptom("Headache");

        // Perform triage
        PriorityLevel priority = engine.TriagePatient(patient);

        Console.WriteLine("=== HOSPITAL TRIAGE SYSTEM ===");
        Console.WriteLine(patient.GetDetails());
        Console.WriteLine("Priority: " + priority);

        // Display next patient
        Patient? next = engine.GetNextPatient();

        if (next != null)
        {
            Console.WriteLine("\nNext patient:");
            Console.WriteLine(next.GetDetails());

            // Dummy staff alert
            engine.AlertStaff(next);
        }

        Console.WriteLine("\nDemo completed.");
    }
}