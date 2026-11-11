namespace HospitalTriage;

// A simple record used by the application history and JSON storage.
public class TriageHistoryEntry
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = "Not specified";
    public int HeartRate { get; set; }
    public int SystolicBP { get; set; }
    public string Symptoms { get; set; } = string.Empty;
    public PriorityLevel Priority { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Status { get; set; } = "Waiting";
    public DateTime? CalledAt { get; set; }
}
