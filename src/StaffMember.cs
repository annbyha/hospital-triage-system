namespace HospitalTriage;

public abstract class StaffMember : Person
{
    public string Role { get; set; }
    public bool IsOnDuty { get; set; }

    public StaffMember(
        int personID, string name, int age, string role)
        : base(personID, name, age)
    {
        Role = role;
        IsOnDuty = true;
    }

    public override string GetDetails()
    {
        return $"{Role}: {Name}";
    }

    public abstract void ReceiveAlert(Patient p);
}