namespace HospitalTriage;
public class StaffMember : Person
{
    public string StaffId { get; set; }

    public StaffMember(string staffId, string name, int age)
        : base(name, age)
    {
        StaffId = staffId;
    }

    public virtual string GetRole()
    {
        return "Staff";
    }

    public override string GetInfo()
    {
        return GetRole() + " " + Name + " (" + StaffId + ")";
    }
}