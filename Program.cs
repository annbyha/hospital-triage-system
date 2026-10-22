
using System;
using System.Collections.Generic;
using System.Text;
using HospitalTriage;

public class Program
{
    const int Width = 50;

    static SevereSymptomRules rules = new SevereSymptomRules();
    static TriageEngine engine = new TriageEngine(rules);
    static PatientQueue queue = new PatientQueue();

    static Doctor doctor = new Doctor(
        "D01", "Dr. Aiman", 40, "Emergency Medicine");

    static Nurse nurse = new Nurse(
        "N01", "Nurse Siti", 30, "Triage Ward");

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        bool running = true;

        while (running)
        {
            ShowMenu();

            int choice = ReadInt("Choose option (1-6): ", 1, 6);

            switch (choice)
            {
                case 1:
                    RegisterPatient();
                    break;
                case 2:
                    ShowQueue();
                    break;
                case 3:
                    CallNextPatient();
                    break;
                case 4:
                    ShowHistory();
                    break;
                case 5:
                    ShowFileHistory();
                    break;
                case 6:
                    running = false;
                    break;
            }

            if (running)
                Pause();
        }

        Console.Clear();
        DrawTop();
        DrawCenter("Thank you for using SRS Clinic!");
        DrawCenter("System closed.");
        DrawBottom();
    }

    // ================ LOGO ================

    static void ShowLogo()
    {
        Console.WriteLine();

        PrintCentered(" *****     *****     *****");
        PrintCentered("**         **   **   **");
        PrintCentered(" *****     *****     *****");
        PrintCentered("     **    **   **       **");
        PrintCentered(" *****     **   **   *****");

        Console.WriteLine();
        PrintCentered("PATIENT TRIAGE CLINIC");
        Console.WriteLine();
    }

    static void PrintCentered(string text)
    {
        int spaces = Math.Max(0, (Width + 2 - text.Length) / 2);
        Console.WriteLine(new string(' ', spaces) + text);
    }

    // ================ MENU ================

    static void ShowMenu()
    {
        Console.Clear();
        ShowLogo();

        DrawTop();
        DrawCenter("SRS PATIENT TRIAGE SYSTEM");
        DrawMid();

        DrawLine("Doctor: " + doctor.GetInfo());
        DrawLine("Nurse: " + nurse.GetInfo());
        DrawLine("Waiting: " + queue.WaitingCount + " patient(s)");
        DrawLine("Registered this session: "
            + queue.GetHistoryList().Count);

        DrawMid();

        DrawLine("1. Register new patient");
        DrawLine("2. View waiting queue");
        DrawLine("3. Call next patient");
        DrawLine("4. View session history");
        DrawLine("5. View saved history file");
        DrawLine("6. Exit");

        DrawBottom();
    }

    // ================ REGISTER PATIENT ================

    static void RegisterPatient()
    {
        Console.Clear();
        ShowLogo();

        DrawTop();
        DrawCenter("REGISTER NEW PATIENT");
        DrawBottom();

        Console.Write("Patient name: ");

        string? inputName = Console.ReadLine();

        string name = string.IsNullOrWhiteSpace(inputName)
            ? "Unknown"
            : inputName.Trim();

        int age = ReadInt("Age: ", 0, 120);

        List<string> symptoms = rules.GetAllSymptoms();

        DrawTop();
        DrawCenter("SELECT SYMPTOM");
        DrawMid();

        for (int i = 0; i < symptoms.Count; i++)
        {
            string line = (i + 1) + ". " + symptoms[i];

            if (rules.IsSevere(symptoms[i]))
                line += " [SEVERE]";

            DrawLine(line);
        }

        DrawBottom();

        int symptomChoice = ReadInt(
            "Symptom number: ", 1, symptoms.Count);

        int hr = ReadInt("Heart rate (bpm): ", 20, 250);
        int sbp = ReadInt("Systolic BP (mmHg): ", 40, 300);

        string id = queue.GenerateId();

        VitalSigns vitals = new VitalSigns(hr, sbp);

        Patient patient = new Patient(
            id, name, age,
            symptoms[symptomChoice - 1],
            vitals);

        patient.Priority = engine.Classify(patient);
        queue.AddPatient(patient);

        Console.WriteLine();

        DrawTop();
        DrawCenter("TRIAGE RESULT");
        DrawMid();

        DrawLine("Patient: " + patient.GetInfo());
        DrawLine("Symptom: " + patient.Symptom);
        DrawLine("Heart rate: " + hr + " bpm");
        DrawLine("Systolic BP: " + sbp + " mmHg");
        DrawLine("Logic: " + engine.GetPQR(patient));
        DrawLine(nurse.CheckPatient(patient));

        Console.ForegroundColor = GetColor(patient.Priority);
        DrawLine("PRIORITY: " + patient.Priority.ToString().ToUpper());
        Console.ResetColor();

        DrawBottom();

        if (patient.Priority == PriorityLevel.Red)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(
                "ALERT: Immediate clinical assessment required!");
            Console.ResetColor();
        }
    }

    // ================ WAITING QUEUE ================

    static void ShowQueue()
    {
        Console.Clear();
        ShowLogo();

        PrintTable(
            "WAITING QUEUE",
            queue.GetWaitingList());
    }

    // ================ CALL NEXT PATIENT ================

    static void CallNextPatient()
    {
        Console.Clear();
        ShowLogo();

        Patient? next = queue.CallNext();

        DrawTop();
        DrawCenter("CALL NEXT PATIENT");
        DrawMid();

        if (next == null)
        {
            DrawLine("No patients in the queue.");
        }
        else
        {
            string message = doctor.Treat(next);

            queue.SaveTreatedToFile(next);

            DrawLine("Calling: " + next.GetInfo());
            DrawLine("Symptom: " + next.Symptom);
            DrawLine("Priority: " + next.Priority);
            DrawLine(message);
        }

        DrawBottom();
    }

    // ================ SESSION HISTORY ================

    static void ShowHistory()
    {
        Console.Clear();
        ShowLogo();

        PrintTable(
            "SESSION HISTORY",
            queue.GetHistoryList());
    }

    // ================ FILE HISTORY ================

    static void ShowFileHistory()
    {
        Console.Clear();
        ShowLogo();

        string[] lines = queue.ReadHistoryFile();

        DrawTop();
        DrawCenter("SAVED HISTORY");
        DrawMid();

        if (lines.Length == 0)
            DrawLine("No records saved yet.");

        foreach (string line in lines)
            DrawLine(line);

        DrawBottom();
    }

    // ================ PATIENT TABLE ================

    static void PrintTable(string title, List<Patient> list)
    {
        DrawTop();
        DrawCenter(title);
        DrawMid();

        DrawLine("ID  Name / Age");
        DrawLine("Symptom");
        DrawLine("HR   SBP   Priority   Status");
        DrawMid();

        if (list.Count == 0)
            DrawLine("No patient records.");

        foreach (Patient p in list)
        {
            DrawLine(p.PatientId + "  " + p.Name + " / " + p.Age);
            DrawLine("Symptom: " + p.Symptom);
            DrawLine(
                "HR: " + p.Vitals.HeartRate +
                "  SBP: " + p.Vitals.SystolicBP +
                "  " + p.Priority + "  " + p.Status);
            DrawMid();
        }

        DrawBottom();
    }

    // ================ BOX DESIGN ================

    static void DrawTop()
    {
        Console.WriteLine("+" + new string('-', Width) + "+");
    }

    static void DrawMid()
    {
        Console.WriteLine("+" + new string('-', Width) + "+");
    }

    static void DrawBottom()
    {
        Console.WriteLine("+" + new string('-', Width) + "+");
    }

    static void DrawLine(string text)
    {
        int maxText = Width - 2;

        if (text.Length > maxText)
            text = text.Substring(0, maxText);

        Console.WriteLine(
            "| " + text.PadRight(maxText) + " |");
    }

    static void DrawCenter(string text)
    {
        if (text.Length > Width)
            text = text.Substring(0, Width);

        int left = (Width - text.Length) / 2;
        int right = Width - text.Length - left;

        Console.WriteLine(
            "|" +
            new string(' ', left) +
            text +
            new string(' ', right) +
            "|");
    }

    // ================ PRIORITY COLOURS ================

    static ConsoleColor GetColor(PriorityLevel level)
    {
        if (level == PriorityLevel.Red)
            return ConsoleColor.Red;

        if (level == PriorityLevel.Yellow)
            return ConsoleColor.Yellow;

        return ConsoleColor.Green;
    }

    // ================ INPUT VALIDATION ================

    static int ReadInt(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);

            string? input = Console.ReadLine();

            if (int.TryParse(input, out int number) &&
                number >= min && number <= max)
            {
                return number;
            }

            Console.WriteLine(
                "Invalid input. Enter a number from " +
                min + " to " + max + ".");
        }
    }

    // ================ PAUSE ================

    static void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to return to menu...");
        Console.ReadLine();
    }
}
