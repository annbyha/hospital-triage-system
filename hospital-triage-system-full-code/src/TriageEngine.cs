using System.Text.Json;

namespace HospitalTriage;

public class TriageEngine
{
    private readonly SevereSymptomRules rules;
    private readonly PatientQueue queue;
    private readonly List<StaffMember> staffList;
    private readonly List<TriageHistoryEntry> sessionHistory;
    private readonly List<TriageHistoryEntry> savedHistory;
    private readonly List<string> alertLog;
    private readonly string historyFilePath;

    public int MinHeartRate { get; private set; } = 50;
    public int MaxHeartRate { get; private set; } = 120;
    public int MinSystolicBP { get; private set; } = 90;
    public int MaxSystolicBP { get; private set; } = 180;

    // True when the current session has changes that are not yet written to history.json.
    public bool HasUnsavedChanges { get; private set; }

    // The most recent patient taken from the queue (shown as "Now serving" in the GUI).
    public Patient? LastCalledPatient { get; private set; }

    public string HistoryFilePath => historyFilePath;
    public SevereSymptomRules Rules => rules;

    public TriageEngine(SevereSymptomRules rules, string? historyFilePath = null)
    {
        this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
        queue = new PatientQueue();
        staffList = new List<StaffMember>();
        sessionHistory = new List<TriageHistoryEntry>();
        savedHistory = new List<TriageHistoryEntry>();
        alertLog = new List<string>();
        this.historyFilePath = historyFilePath ?? Path.Combine(AppContext.BaseDirectory, "history.json");
        LoadHistory();
    }

    // ---------------- Staff ----------------

    public void RegisterStaff(StaffMember staff)
    {
        ArgumentNullException.ThrowIfNull(staff);
        if (staffList.Any(s => s.PersonID == staff.PersonID))
            throw new InvalidOperationException($"Staff ID {staff.PersonID} is already registered.");
        staffList.Add(staff);
    }

    public bool RemoveStaff(int staffId) => staffList.RemoveAll(s => s.PersonID == staffId) > 0;

    public List<StaffMember> GetStaff() => staffList.ToList();
    public List<StaffMember> GetOnDutyStaff() => staffList.Where(s => s.IsOnDuty).ToList();

    public int GetNextStaffId() => staffList.Count == 0 ? 101 : staffList.Max(s => s.PersonID) + 1;

    // ---------------- Triage rules ----------------

    public bool EvaluateP(Patient patient) => rules.IsSevere(patient.Symptoms);
    public bool EvaluateP(IEnumerable<string> symptoms) => rules.IsSevere(symptoms);
    public bool EvaluateQ(VitalSigns vitals) => vitals.HeartRate < MinHeartRate || vitals.HeartRate > MaxHeartRate;
    public bool EvaluateR(VitalSigns vitals) => vitals.SystolicBP < MinSystolicBP || vitals.SystolicBP > MaxSystolicBP;

    public PriorityLevel Classify(bool p, bool q, bool r)
    {
        if (p || (q && r)) return PriorityLevel.Red;
        if (q || r) return PriorityLevel.Yellow;
        return PriorityLevel.Green;
    }

    // Lets the GUI preview the result before the patient is actually registered.
    public PriorityLevel PreviewPriority(IEnumerable<string> symptoms, VitalSigns vitals) =>
        Classify(EvaluateP(symptoms), EvaluateQ(vitals), EvaluateR(vitals));

    // Human-readable explanation of why a patient received their priority.
    public List<string> ExplainTriage(IEnumerable<string> symptoms, VitalSigns vitals)
    {
        var reasons = new List<string>();
        var severe = symptoms.Where(rules.IsSevereSymptom).ToList();
        if (severe.Count > 0)
            reasons.Add($"P - Severe symptom present: {string.Join(", ", severe)}");
        if (EvaluateQ(vitals))
            reasons.Add($"Q - Abnormal heart rate: {vitals.HeartRate} bpm (normal {MinHeartRate}-{MaxHeartRate})");
        if (EvaluateR(vitals))
            reasons.Add($"R - Abnormal systolic BP: {vitals.SystolicBP} mmHg (normal {MinSystolicBP}-{MaxSystolicBP})");
        if (reasons.Count == 0)
            reasons.Add("No severe symptoms and vital signs are within the normal range.");
        return reasons;
    }

    public void UpdateThresholds(int minHeartRate, int maxHeartRate, int minSystolicBP, int maxSystolicBP)
    {
        if (minHeartRate <= 0 || minSystolicBP <= 0)
            throw new ArgumentOutOfRangeException(nameof(minHeartRate), "Thresholds must be positive.");
        if (minHeartRate >= maxHeartRate)
            throw new ArgumentException("Minimum heart rate must be lower than the maximum heart rate.");
        if (minSystolicBP >= maxSystolicBP)
            throw new ArgumentException("Minimum systolic BP must be lower than the maximum systolic BP.");
        MinHeartRate = minHeartRate;
        MaxHeartRate = maxHeartRate;
        MinSystolicBP = minSystolicBP;
        MaxSystolicBP = maxSystolicBP;
    }

    // ---------------- Patients ----------------

    public bool IsPatientIdInUse(int patientId) =>
        sessionHistory.Any(h => h.PatientId == patientId) || queue.ContainsPatient(patientId);

    public int GetNextPatientId()
    {
        int maxId = sessionHistory.Select(h => h.PatientId)
            .Concat(savedHistory.Select(h => h.PatientId))
            .DefaultIfEmpty(1000)
            .Max();
        return Math.Max(maxId, 1000) + 1;
    }

    public PriorityLevel TriagePatient(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        if (!patient.Vitals.IsValid())
            throw new InvalidOperationException("Vital-sign values must be positive.");
        if (IsPatientIdInUse(patient.PersonID))
            throw new InvalidOperationException($"Patient ID {patient.PersonID} has already been registered in this session.");

        patient.Priority = Classify(EvaluateP(patient), EvaluateQ(patient.Vitals), EvaluateR(patient.Vitals));
        queue.Enqueue(patient);
        sessionHistory.Add(CreateHistoryEntry(patient));
        HasUnsavedChanges = true;
        return patient.Priority;
    }

    // Notifies every on-duty staff member and returns the messages that were sent.
    public List<string> AlertStaff(Patient patient)
    {
        var messages = new List<string>();
        foreach (StaffMember staff in staffList.Where(s => s.IsOnDuty))
        {
            string message = staff.ReceiveAlert(patient);
            messages.Add(message);
            alertLog.Add($"[{DateTime.Now:dd/MM HH:mm:ss}] {message}");
        }
        if (messages.Count == 0)
            alertLog.Add($"[{DateTime.Now:dd/MM HH:mm:ss}] WARNING: RED patient {patient.Name} registered but no staff are on duty!");
        return messages;
    }

    public IReadOnlyList<string> GetAlertLog() => alertLog.AsReadOnly();

    public Patient? GetNextPatient() => queue.PeekNext();
    public Patient? PeekNextPatient() => queue.PeekNext();
    public List<Patient> GetWaitingPatients() => queue.GetAll();
    public List<Patient> GetWaitingPatients(PriorityLevel level) => queue.GetByPriority(level);
    public int GetWaitingCount() => queue.Count();
    public int GetWaitingCount(PriorityLevel level) => queue.GetByPriority(level).Count;

    public Patient? CallNextPatient()
    {
        Patient? patient = queue.DequeueNext();
        if (patient != null)
        {
            RecordPatientCalled(patient);
            LastCalledPatient = patient;
            HasUnsavedChanges = true;
        }
        return patient;
    }

    public void RecordPatientCalled(Patient patient)
    {
        TriageHistoryEntry? entry = sessionHistory.LastOrDefault(h => h.PatientId == patient.PersonID);
        if (entry == null) return;
        entry.Status = "Called";
        entry.CalledAt = DateTime.Now;
    }

    // ---------------- Statistics ----------------

    public int GetCalledCount() => sessionHistory.Count(h => h.Status == "Called");

    public TimeSpan GetAverageWaitingTime()
    {
        var waits = queue.GetAll().Select(p => p.GetWaitingDuration()).ToList();
        return waits.Count == 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(waits.Average(w => w.TotalSeconds));
    }

    // ---------------- History ----------------

    public IReadOnlyList<TriageHistoryEntry> GetSessionHistory() => sessionHistory.AsReadOnly();
    public IReadOnlyList<TriageHistoryEntry> GetSavedHistory() => savedHistory.AsReadOnly();

    public void SaveHistory()
    {
        string? directory = Path.GetDirectoryName(historyFilePath);
        if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);

        // Merge current session records with existing saved records by patient ID and registration time.
        var merged = savedHistory
            .Concat(sessionHistory)
            .GroupBy(h => new { h.PatientId, h.RegisteredAt })
            .Select(g => g.Last())
            .OrderByDescending(h => h.RegisteredAt)
            .ToList();

        string json = JsonSerializer.Serialize(merged, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(historyFilePath, json);
        savedHistory.Clear();
        savedHistory.AddRange(merged);
        HasUnsavedChanges = false;
    }

    public void LoadHistory()
    {
        if (!File.Exists(historyFilePath)) return;
        try
        {
            string json = File.ReadAllText(historyFilePath);
            List<TriageHistoryEntry>? records = JsonSerializer.Deserialize<List<TriageHistoryEntry>>(json);
            if (records == null) return;
            savedHistory.Clear();
            savedHistory.AddRange(records.OrderByDescending(h => h.RegisteredAt));
        }
        catch (JsonException)
        {
            // Keep the application usable if the history file is damaged or not valid JSON.
            savedHistory.Clear();
        }
        catch (IOException)
        {
            savedHistory.Clear();
        }
    }

    public void ClearSavedHistory()
    {
        savedHistory.Clear();
        if (File.Exists(historyFilePath)) File.Delete(historyFilePath);
    }

    private static TriageHistoryEntry CreateHistoryEntry(Patient patient) => new()
    {
        PatientId = patient.PersonID,
        PatientName = patient.Name,
        Age = patient.Age,
        Gender = patient.Gender,
        HeartRate = patient.Vitals.HeartRate,
        SystolicBP = patient.Vitals.SystolicBP,
        Symptoms = string.Join(", ", patient.Symptoms),
        Priority = patient.Priority,
        RegisteredAt = patient.ArrivalTime,
        Status = "Waiting"
    };
}
