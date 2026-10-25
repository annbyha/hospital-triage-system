using System;
using System.Collections.Generic;

namespace HospitalTriage;

public class SevereSymptomRules
{
    private List<string> severeSymptoms = new List<string>();

    public SevereSymptomRules()
    {
        severeSymptoms.Add("Difficulty breathing");
        severeSymptoms.Add("Chest pain");
        severeSymptoms.Add("Unconsciousness");
    }

    public void AddSevereSymptoms(string symptom)
    {
        if (!severeSymptoms.Contains(symptom))
        {
            severeSymptoms.Add(symptom);
        }
    }

    public bool IsSevere(List<string> symptoms)
    {
        foreach (string symptom in symptoms)
        {
            foreach (string severe in severeSymptoms)
            {
                if (string.Equals(
                    symptom, severe,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }
}