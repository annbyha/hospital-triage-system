using System.Collections.Generic;
namespace HospitalTriage;
public class SevereSymptomRules
{
    private List<string> severeSymptoms;
    private List<string> mildSymptoms;

    public SevereSymptomRules()
    {
        severeSymptoms = new List<string>();
        severeSymptoms.Add("Difficulty breathing");
        severeSymptoms.Add("Chest pain");
        severeSymptoms.Add("Unconsciousness");

        mildSymptoms = new List<string>();
        mildSymptoms.Add("Fever");
        mildSymptoms.Add("Headache");
        mildSymptoms.Add("Cough");
        mildSymptoms.Add("Minor injury");
    }

    // Proposition P
    public bool IsSevere(string symptom)
    {
        return severeSymptoms.Contains(symptom);
    }

    public List<string> GetAllSymptoms()
    {
        List<string> all = new List<string>();
        all.AddRange(severeSymptoms);
        all.AddRange(mildSymptoms);
        return all;
    }
}