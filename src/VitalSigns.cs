namespace HospitalTriage;

/// propositions Q and R
public class VitalSigns
{
    public int HeartRate { get; }
    public int SystolicBP { get; }

    public VitalSigns(int heartRate, int systolicBP)
    {
        HeartRate = heartRate;
        SystolicBP = systolicBP;
    }

    /// HR &lt; 50 or HR &gt; 120
    public bool IsHeartRateAbnormal() => HeartRate < 50 || HeartRate > 120;

    /// SBP &lt; 90 or SBP &gt; 180
    public bool IsBloodPressureAbnormal() => SystolicBP < 90 || SystolicBP > 180;
}
