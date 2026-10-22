namespace HospitalTriage;
public class Nurse : StaffMember
{
    public string Ward { get; set; }

    public Nurse(string staffId, string name, int age, string ward)
        : base(staffId, name, age)
    {
        Ward = ward;
    }

    public override string GetRole()
    {
        return "Nurse";
    }


    public string CheckPatient(Patient patient)
    {
        return Name + " checked vitals of " + patient.Name + " at " + Ward;
    }
}