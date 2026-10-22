namespace HospitalTriage;
public class VitalSigns
{
    public int HeartRate { get; set; }
    public int SystolicBP { get; set; }

    public VitalSigns(int heartRate, int systolicBP)
    {
        HeartRate = heartRate;
        SystolicBP = systolicBP;
    }

    // Proposition Q
    public bool IsHeartRateAbnormal()
    {
        return HeartRate < 50 || HeartRate > 120;
    }

    // Proposition R
    public bool IsBloodPressureAbnormal()
    {
        return SystolicBP < 90 || SystolicBP > 180;
    }
}