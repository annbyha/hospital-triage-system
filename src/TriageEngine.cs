namespace HospitalTriage;
public class TriageEngine
{
    private SevereSymptomRules rules;

    public TriageEngine(SevereSymptomRules rules)
    {
        this.rules = rules;
    }

    public PriorityLevel Classify(Patient patient)
    {
        bool p = rules.IsSevere(patient.Symptom);
        bool q = patient.Vitals.IsHeartRateAbnormal();
        bool r = patient.Vitals.IsBloodPressureAbnormal();

        if (p)
        {
            return PriorityLevel.Red;       // Red = P
        }
        else if (q && r)
        {
            return PriorityLevel.Red;       // Red = Q AND R
        }
        else if (q || r)
        {
            return PriorityLevel.Yellow;    // Yellow = NOT P AND (Q OR R)
        }
        else
        {
            return PriorityLevel.Green;     // Green = NOT P AND NOT Q AND NOT R
        }
    }

    
    public string GetPQR(Patient patient)
    {
        bool p = rules.IsSevere(patient.Symptom);
        bool q = patient.Vitals.IsHeartRateAbnormal();
        bool r = patient.Vitals.IsBloodPressureAbnormal();

        return "P=" + (p ? "T" : "F") + "  Q=" + (q ? "T" : "F") + "  R=" + (r ? "T" : "F");
    }
}