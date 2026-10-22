namespace HospitalTriage;
public class Doctor : StaffMember
{
    public string Specialty { get; set; }

    public Doctor(string staffId, string name, int age, string specialty)
        : base(staffId, name, age)
    {
        Specialty = specialty;
    }

    public override string GetRole()
    {
        return "Doctor";
    }

    
    public string Treat(Patient patient)
    {
        patient.Status = "Treated";
        patient.TreatedBy = Name;
        return Name + " (" + Specialty + ") is treating " + patient.Name;
    }
}