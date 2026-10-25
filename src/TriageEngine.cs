using System.Collections.Generic;

namespace HospitalTriage;

public class TriageEngine
{
    private SevereSymptomRules rules;
    private PatientQueue queue;
    private List<StaffMember> staffList;

    public int MinHeartRate = 50;
    public int MaxHeartRate = 120;
    public int MinSystolicBP = 90;
    public int MaxSystolicBP = 180;

    public TriageEngine(SevereSymptomRules rules)
    {
        this.rules = rules;
        queue = new PatientQueue();
        staffList = new List<StaffMember>();
    }

    public void RegisterStaff(StaffMember staff)
    {
        staffList.Add(staff);
    }

    public bool EvaluateP(Patient p)
    {
        return rules.IsSevere(p.Symptoms);
    }

    public bool EvaluateQ(VitalSigns v)
    {
        return v.HeartRate < MinHeartRate ||
               v.HeartRate > MaxHeartRate;
    }

    public bool EvaluateR(VitalSigns v)
    {
        return v.SystolicBP < MinSystolicBP ||
               v.SystolicBP > MaxSystolicBP;
    }

    public PriorityLevel Classify(bool p, bool q, bool r)
    {
        if (p || (q && r))
            return PriorityLevel.Red;

        if (q || r)
            return PriorityLevel.Yellow;

        return PriorityLevel.Green;
    }

    public PriorityLevel TriagePatient(Patient patient)
    {
        patient.Priority = Classify(
            EvaluateP(patient),
            EvaluateQ(patient.Vitals),
            EvaluateR(patient.Vitals));

        queue.EnterQueue(patient);

        return patient.Priority;
    }

    public void AlertStaff(Patient patient)
    {
        foreach (StaffMember staff in staffList)
        {
            if (staff.IsOnDuty)
            {
                staff.ReceiveAlert(patient);
            }
        }
    }

    public Patient? GetNextPatient()
    {
        return queue.PeekNext();
    }
}