namespace HospitalTriage;

public class Patient : Person
{
    public List<string> Symptoms { get; set; }
    public VitalSigns Vitals { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Gender { get; set; } = "Not specified";

    public Patient(int patientId, string name, int age, VitalSigns vitals)
        : base(patientId, name, age)
    {
        Vitals = vitals ?? throw new ArgumentNullException(nameof(vitals));
        Symptoms = new List<string>();
        Priority = PriorityLevel.Green;
        ArrivalTime = DateTime.Now;
    }

    public void AddSymptom(string symptom)
    {
        if (string.IsNullOrWhiteSpace(symptom)) return;
        string cleanSymptom = symptom.Trim();
        if (!Symptoms.Contains(cleanSymptom, StringComparer.OrdinalIgnoreCase))
            Symptoms.Add(cleanSymptom);
    }

    // Kept for compatibility with the original dummy method.
    public DateTime GetWaitingTime() => ArrivalTime;

    public TimeSpan GetWaitingDuration() => DateTime.Now - ArrivalTime;

    public override string GetDetails()
    {
        string symptoms = Symptoms.Count == 0 ? "None recorded" : string.Join(", ", Symptoms);
        return $"Patient ID: {PersonID}, Name: {Name}, Age: {Age}, Gender: {Gender}, Priority: {Priority}, " +
               $"Heart Rate: {Vitals.HeartRate} bpm, Systolic BP: {Vitals.SystolicBP} mmHg, Symptoms: {symptoms}";
    }
}
