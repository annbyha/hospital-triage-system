namespace HospitalTriage;

public class SevereSymptomRules
{
    private readonly List<string> severeSymptoms = new()
    {
        "Difficulty breathing",
        "Chest pain",
        "Unconsciousness"
    };

    public IReadOnlyList<string> GetSevereSymptoms() => severeSymptoms.AsReadOnly();

    public void AddSevereSymptom(string symptom)
    {
        if (string.IsNullOrWhiteSpace(symptom))
            throw new ArgumentException("Symptom cannot be empty.", nameof(symptom));
        if (!severeSymptoms.Contains(symptom.Trim(), StringComparer.OrdinalIgnoreCase))
            severeSymptoms.Add(symptom.Trim());
    }

    public bool RemoveSevereSymptom(string symptom)
    {
        int index = severeSymptoms.FindIndex(s => s.Equals(symptom?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;
        severeSymptoms.RemoveAt(index);
        return true;
    }

    public bool IsSevereSymptom(string symptom) =>
        severeSymptoms.Contains(symptom?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);

    // Original dummy method name retained as a wrapper.
    public void AddSevereSymptoms(string symptom) => AddSevereSymptom(symptom);

    public bool IsSevere(IEnumerable<string> symptoms)
    {
        return symptoms.Any(symptom => severeSymptoms.Contains(symptom, StringComparer.OrdinalIgnoreCase));
    }
}
