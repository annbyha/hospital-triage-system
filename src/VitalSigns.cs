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

    public bool IsValid()
    {
        return HeartRate >= 0 && SystolicBP >= 0;
    }
}