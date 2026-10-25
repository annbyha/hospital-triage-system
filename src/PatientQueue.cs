using System.Collections.Generic;
using System.Linq;

namespace HospitalTriage;

public class PatientQueue
{
    private List<Patient> patients = new List<Patient>();

    public void EnterQueue(Patient p)
    {
        patients.Add(p);
    }

    public Patient? DeleteQueueNext()
    {
        Patient? next = PeekNext();

        if (next != null)
        {
            patients.Remove(next);
        }

        return next;
    }

    public Patient? PeekNext()
    {
        return patients
            .OrderBy(p => p.Priority)
            .ThenBy(p => p.ArrivalTime)
            .FirstOrDefault();
    }

    public List<Patient> GetByPriority(PriorityLevel level)
    {
        return patients
            .Where(p => p.Priority == level)
            .ToList();
    }

    public int Count()
    {
        return patients.Count;
    }
}