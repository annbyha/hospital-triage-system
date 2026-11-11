namespace HospitalTriage;

public class PatientQueue
{
    private readonly List<Patient> patients = new();

    public void Enqueue(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        if (patients.Any(p => p.PersonID == patient.PersonID))
            throw new InvalidOperationException($"Patient ID {patient.PersonID} is already in the waiting queue.");
        patients.Add(patient);
    }

    public Patient? DequeueNext()
    {
        Patient? next = PeekNext();
        if (next != null) patients.Remove(next);
        return next;
    }

    public Patient? PeekNext() => patients
        .OrderBy(p => p.Priority)
        .ThenBy(p => p.ArrivalTime)
        .FirstOrDefault();

    public List<Patient> GetByPriority(PriorityLevel level) =>
        patients.Where(p => p.Priority == level).OrderBy(p => p.ArrivalTime).ToList();

    public List<Patient> GetAll() => patients
        .OrderBy(p => p.Priority).ThenBy(p => p.ArrivalTime).ToList();

    public bool ContainsPatient(int patientId) => patients.Any(p => p.PersonID == patientId);
    public int Count() => patients.Count;

    // Compatibility wrappers for names used in the original dummy.
    public void EnterQueue(Patient patient) => Enqueue(patient);
    public Patient? DeleteQueueNext() => DequeueNext();
}
