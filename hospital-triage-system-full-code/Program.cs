using HospitalTriage;

// Console-based test interface for the completed system logic.
// This will later be replaced by the Windows Forms startup code.
internal class Program
{
    private static void Main()
    {
        var rules = new SevereSymptomRules();
        var engine = new TriageEngine(rules);
        engine.RegisterStaff(new Nurse(101, "Siti", 30, "Emergency Ward"));
        engine.RegisterStaff(new Doctor(102, "Aiman", 40, "Emergency Medicine"));

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n===== HOSPITAL PATIENT TRIAGE SYSTEM =====");
            Console.WriteLine("1. Register and triage patient");
            Console.WriteLine("2. View waiting queue");
            Console.WriteLine("3. Call next patient");
            Console.WriteLine("4. View current session history");
            Console.WriteLine("5. View saved history");
            Console.WriteLine("6. Save history");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            switch (Console.ReadLine())
            {
                case "1": RegisterPatient(engine); break;
                case "2": DisplayQueue(engine); break;
                case "3": CallNextPatient(engine); break;
                case "4": DisplayHistory(engine.GetSessionHistory()); break;
                case "5": DisplayHistory(engine.GetSavedHistory()); break;
                case "6":
                    engine.SaveHistory();
                    Console.WriteLine("History saved.");
                    break;
                case "0":
                    engine.SaveHistory();
                    running = false;
                    break;
                default: Console.WriteLine("Invalid option. Please try again."); break;
            }
        }
    }

    private static void RegisterPatient(TriageEngine engine)
    {
        Console.Write("Patient ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
        {
            Console.WriteLine("Please enter a positive numeric patient ID.");
            return;
        }

        Console.Write("Full name: ");
        string name = (Console.ReadLine() ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty.");
            return;
        }

        Console.Write("Age: ");
        if (!int.TryParse(Console.ReadLine(), out int age) || age < 0 || age > 130)
        {
            Console.WriteLine("Please enter an age between 0 and 130.");
            return;
        }

        Console.Write("Heart rate (bpm): ");
        if (!int.TryParse(Console.ReadLine(), out int heartRate) || heartRate <= 0)
        {
            Console.WriteLine("Please enter a positive heart-rate value.");
            return;
        }

        Console.Write("Systolic blood pressure (mmHg): ");
        if (!int.TryParse(Console.ReadLine(), out int systolicBP) || systolicBP <= 0)
        {
            Console.WriteLine("Please enter a positive systolic blood-pressure value.");
            return;
        }

        Console.WriteLine("Enter symptoms separated by commas (or leave blank):");
        string symptomInput = Console.ReadLine() ?? string.Empty;
        var patient = new Patient(id, name, age, new VitalSigns(heartRate, systolicBP));
        foreach (string symptom in symptomInput.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            patient.AddSymptom(symptom);

        try
        {
            PriorityLevel priority = engine.TriagePatient(patient);
            Console.WriteLine($"Patient registered. Triage category: {priority}");
            if (priority == PriorityLevel.Red)
                engine.AlertStaff(patient);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static void DisplayQueue(TriageEngine engine)
    {
        List<Patient> patients = engine.GetWaitingPatients();
        if (patients.Count == 0)
        {
            Console.WriteLine("There are no patients in the waiting queue.");
            return;
        }

        Console.WriteLine("\n--- WAITING QUEUE (Red, Yellow, Green) ---");
        foreach (Patient patient in patients)
            Console.WriteLine($"{patient.PersonID} | {patient.Name} | Age {patient.Age} | {patient.Priority} | Waiting: {patient.GetWaitingDuration():hh\\:mm\\:ss}");
    }

    private static void CallNextPatient(TriageEngine engine)
    {
        Patient? patient = engine.CallNextPatient();
        if (patient == null)
        {
            Console.WriteLine("No patients are waiting.");
            return;
        }
        Console.WriteLine($"Now calling: {patient.Name} (ID: {patient.PersonID}, Priority: {patient.Priority})");
    }

    private static void DisplayHistory(IReadOnlyList<TriageHistoryEntry> entries)
    {
        if (entries.Count == 0)
        {
            Console.WriteLine("No history records found.");
            return;
        }
        Console.WriteLine("\n--- PATIENT HISTORY ---");
        foreach (TriageHistoryEntry entry in entries)
            Console.WriteLine($"{entry.PatientId} | {entry.PatientName} | {entry.Priority} | Registered: {entry.RegisteredAt:g} | Status: {entry.Status}");
    }
}
