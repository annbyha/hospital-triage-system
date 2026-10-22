
using System;
using System.IO;
using System.Collections.Generic;

namespace HospitalTriage;

public class PatientQueue
{
    private readonly List<Patient> waitingList;
    private readonly List<Patient> historyList;
    private int counter;

    // Save history.txt in the current working directory.
    private readonly string fileName =
        Path.Combine(Directory.GetCurrentDirectory(), "history.txt");

    public PatientQueue()
    {
        waitingList = new List<Patient>();
        historyList = new List<Patient>();

        // Continue the ID number from previously saved records.
        counter = GetLastPatientNumber();
    }

    public int WaitingCount
    {
        get { return waitingList.Count; }
    }

    // Generate IDs without repeating IDs found in history.txt.
    public string GenerateId()
    {
        counter++;
        return "P" + counter.ToString("000");
    }

    // Register patient in the current session and save the record.
    public void AddPatient(Patient patient)
    {
        waitingList.Add(patient);
        historyList.Add(patient);

        SortWaitingList();
        SaveRegisteredToFile(patient);
    }

    // Remove the most urgent patient from the waiting queue.
    // The doctor will update the treatment status.
    public Patient? CallNext()
    {
        if (waitingList.Count == 0)
        {
            return null;
        }

        Patient next = waitingList[0];
        waitingList.RemoveAt(0);

        return next;
    }

    public List<Patient> GetWaitingList()
    {
        return waitingList;
    }

    // Contains patients registered during this session only.
    public List<Patient> GetHistoryList()
    {
        return historyList;
    }

    // Find the largest patient number already saved in the file.
    private int GetLastPatientNumber()
    {
        int maxNumber = 0;

        if (!File.Exists(fileName))
        {
            return maxNumber;
        }

        string[] lines = File.ReadAllLines(fileName);

        foreach (string line in lines)
        {
            string[] parts = line.Split('|');

            // Expected record format:
            // date | REG | P001 | patient name | ...
            if (parts.Length < 3)
            {
                continue;
            }

            string recordType = parts[1].Trim();
            string patientId = parts[2].Trim();

            if (recordType == "REG" &&
                patientId.StartsWith("P") &&
                int.TryParse(patientId.Substring(1), out int number))
            {
                if (number > maxNumber)
                {
                    maxNumber = number;
                }
            }
        }

        return maxNumber;
    }

    // Append registration record; never overwrite existing history.
    private void SaveRegisteredToFile(Patient patient)
    {
        string line =
            DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
            " | REG | " +
            patient.PatientId + " | " +
            patient.Name + " | Age: " +
            patient.Age + " | Symptom: " +
            patient.Symptom + " | HR: " +
            patient.Vitals.HeartRate + " | SBP: " +
            patient.Vitals.SystolicBP + " | Priority: " +
            patient.Priority;

        File.AppendAllText(fileName, line + Environment.NewLine);
    }

    // Save treatment record after the doctor treats the patient.
    public void SaveTreatedToFile(Patient patient)
    {
        string line =
            DateTime.Now.ToString("dd/MM/yyyy HH:mm") +
            " | TREATED | " +
            patient.PatientId + " | " +
            patient.Name + " | Treated by: " +
            patient.TreatedBy;

        File.AppendAllText(fileName, line + Environment.NewLine);
    }

    // Read all saved records for menu option 5.
    public string[] ReadHistoryFile()
    {
        if (File.Exists(fileName))
        {
            return File.ReadAllLines(fileName);
        }

        return Array.Empty<string>();
    }

    // Sort by priority first, then by arrival time.
    // Red (1) comes before Yellow (2), then Green (3).
    private void SortWaitingList()
    {
        for (int i = 0; i < waitingList.Count - 1; i++)
        {
            for (int j = 0; j < waitingList.Count - 1 - i; j++)
            {
                Patient a = waitingList[j];
                Patient b = waitingList[j + 1];

                bool swap = false;

                if ((int)a.Priority > (int)b.Priority)
                {
                    swap = true;
                }
                else if (
                    a.Priority == b.Priority &&
                    a.ArrivalTime > b.ArrivalTime)
                {
                    swap = true;
                }

                if (swap)
                {
                    waitingList[j] = b;
                    waitingList[j + 1] = a;
                }
            }
        }
    }
}
